using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.OpenClaw;

public interface IOpenClawAssistantService
{
    Task<string> GenerateSummaryAsync(RequestEntity request, CancellationToken cancellationToken);
    Task<string> DraftCommentAsync(RequestEntity request, string intent, CancellationToken cancellationToken);
}
