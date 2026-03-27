using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.OpenClaw;

public class OpenClawAssistantService(HttpClient httpClient, ILogger<OpenClawAssistantService> logger) : IOpenClawAssistantService
{
    public async Task<string> GenerateSummaryAsync(RequestEntity request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/assistant/summary", new
            {
                requestId = request.Id,
                request.Title,
                request.Description,
                status = request.Status.ToString()
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<AssistantResponse>(cancellationToken);
                if (!string.IsNullOrWhiteSpace(payload?.Result))
                {
                    return payload.Result;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "OpenClaw summary call failed. Using fallback response.");
        }

        return $"Request '{request.Title}' is currently '{request.Status}'. Description: {request.Description}";
    }

    public async Task<string> DraftCommentAsync(RequestEntity request, string intent, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/assistant/draft-comment", new
            {
                requestId = request.Id,
                intent,
                request.Title,
                request.Description,
                status = request.Status.ToString()
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<AssistantResponse>(cancellationToken);
                if (!string.IsNullOrWhiteSpace(payload?.Result))
                {
                    return payload.Result;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "OpenClaw draft comment call failed. Using fallback response.");
        }

        return $"Please review '{request.Title}'. Suggested intent: {intent}.";
    }

    private sealed record AssistantResponse(string Result);
}
