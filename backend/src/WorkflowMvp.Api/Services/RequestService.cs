using Microsoft.EntityFrameworkCore;
using WorkflowMvp.Api.Camunda;
using WorkflowMvp.Api.Data;
using WorkflowMvp.Api.Models;
using WorkflowMvp.Api.OpenClaw;

namespace WorkflowMvp.Api.Services;

public class RequestService(
    AppDbContext dbContext,
    ICamundaWorkflowClient camundaWorkflowClient,
    IOpenClawAssistantService openClawAssistantService) : IRequestService
{
    public async Task<RequestDetailDto> CreateAsync(CreateRequestDto input, string actor, CancellationToken cancellationToken)
    {
        var request = new RequestEntity
        {
            Title = input.Title,
            Description = input.Description,
            CreatedBy = actor,
            Status = RequestStatus.PendingManagerApproval
        };

        dbContext.Requests.Add(request);
        dbContext.AuditLogs.Add(new AuditLogEntity
        {
            RequestId = request.Id,
            Action = "Created",
            PerformedBy = actor,
            Details = "Request created and moved to manager approval."
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        request.CamundaProcessInstanceKey = await camundaWorkflowClient.StartRequestProcessAsync(request, cancellationToken);
        request.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetAsync(request.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to read created request.");
    }

    public async Task<IReadOnlyCollection<RequestListItemDto>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Requests
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new RequestListItemDto(
                x.Id,
                x.Title,
                x.Description,
                x.CreatedBy,
                x.Status.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<RequestDetailDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await dbContext.Requests
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var logs = await dbContext.AuditLogs
            .Where(x => x.RequestId == id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new AuditLogDto(x.Action, x.PerformedBy, x.Details, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new RequestDetailDto(
            request.Id,
            request.Title,
            request.Description,
            request.CreatedBy,
            request.Status.ToString(),
            request.CamundaProcessInstanceKey,
            request.CreatedAt,
            request.UpdatedAt,
            request.Comments
                .OrderBy(x => x.CreatedAt)
                .Select(x => new RequestCommentDto(x.Author, x.Comment, x.CreatedAt))
                .ToList(),
            logs);
    }

    public Task<RequestDetailDto?> ApproveAsync(Guid id, string actor, string role, string? comment, CancellationToken cancellationToken)
        => UpdateStatusAsync(id, actor, role, comment, isApprove: true, cancellationToken);

    public Task<RequestDetailDto?> RejectAsync(Guid id, string actor, string role, string? comment, CancellationToken cancellationToken)
        => UpdateStatusAsync(id, actor, role, comment, isApprove: false, cancellationToken);

    public async Task<RequestSummaryDto?> GetSummaryAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await dbContext.Requests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (request is null)
        {
            return null;
        }

        var summary = await openClawAssistantService.GenerateSummaryAsync(request, cancellationToken);
        return new RequestSummaryDto(request.Id, request.Status.ToString(), summary);
    }

    private async Task<RequestDetailDto?> UpdateStatusAsync(
        Guid id,
        string actor,
        string role,
        string? comment,
        bool isApprove,
        CancellationToken cancellationToken)
    {
        var request = await dbContext.Requests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (request is null)
        {
            return null;
        }

        var nextStatus = request.Status switch
        {
            RequestStatus.PendingManagerApproval when role == "manager" && isApprove => RequestStatus.PendingFinanceApproval,
            RequestStatus.PendingFinanceApproval when role == "finance" && isApprove => RequestStatus.Approved,
            RequestStatus.PendingManagerApproval when role == "manager" && !isApprove => RequestStatus.Rejected,
            RequestStatus.PendingFinanceApproval when role == "finance" && !isApprove => RequestStatus.Rejected,
            _ => throw new InvalidOperationException("Invalid transition for the current role or status.")
        };

        request.Status = nextStatus;
        request.UpdatedAt = DateTimeOffset.UtcNow;

        if (!string.IsNullOrWhiteSpace(comment))
        {
            dbContext.RequestComments.Add(new RequestCommentEntity
            {
                RequestId = request.Id,
                Author = actor,
                Comment = comment
            });
        }

        dbContext.AuditLogs.Add(new AuditLogEntity
        {
            RequestId = request.Id,
            Action = isApprove ? "Approved" : "Rejected",
            PerformedBy = actor,
            Details = $"Role '{role}' changed status to '{nextStatus}'."
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetAsync(id, cancellationToken);
    }
}
