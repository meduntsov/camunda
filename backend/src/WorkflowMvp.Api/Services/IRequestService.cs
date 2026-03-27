using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.Services;

public interface IRequestService
{
    Task<RequestDetailDto> CreateAsync(CreateRequestDto input, string actor, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<RequestListItemDto>> ListAsync(CancellationToken cancellationToken);
    Task<RequestDetailDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestDetailDto?> ApproveAsync(Guid id, string actor, string role, string? comment, CancellationToken cancellationToken);
    Task<RequestDetailDto?> RejectAsync(Guid id, string actor, string role, string? comment, CancellationToken cancellationToken);
    Task<RequestSummaryDto?> GetSummaryAsync(Guid id, CancellationToken cancellationToken);
}
