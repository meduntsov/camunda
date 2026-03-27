using Microsoft.AspNetCore.Mvc;
using WorkflowMvp.Api.Models;
using WorkflowMvp.Api.OpenClaw;
using WorkflowMvp.Api.Services;

namespace WorkflowMvp.Api.Controllers;

[ApiController]
[Route("api/assistant")]
public class AssistantController(IRequestService requestService, IOpenClawAssistantService openClawAssistantService) : ControllerBase
{
    [HttpGet("requests/{id:guid}/status")]
    public async Task<ActionResult<object>> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        var request = await requestService.GetAsync(id, cancellationToken);
        return request is null ? NotFound() : Ok(new { request.Id, request.Status, request.UpdatedAt });
    }

    [HttpGet("requests/{id:guid}/summary")]
    public async Task<ActionResult<RequestSummaryDto>> GetSummary(Guid id, CancellationToken cancellationToken)
    {
        var summary = await requestService.GetSummaryAsync(id, cancellationToken);
        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpPost("requests/{id:guid}/draft-comment")]
    public async Task<ActionResult<object>> DraftComment(Guid id, [FromBody] DraftCommentInput input, CancellationToken cancellationToken)
    {
        var request = await requestService.GetAsync(id, cancellationToken);
        if (request is null)
        {
            return NotFound();
        }

        var entity = new RequestEntity
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Status = Enum.TryParse<RequestStatus>(request.Status, out var status) ? status : RequestStatus.Draft
        };

        var draft = await openClawAssistantService.DraftCommentAsync(entity, input.Intent ?? "General review", cancellationToken);
        return Ok(new { requestId = id, draft });
    }

    public sealed record DraftCommentInput(string? Intent);
}
