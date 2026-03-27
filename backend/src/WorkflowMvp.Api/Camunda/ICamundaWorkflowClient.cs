using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.Camunda;

public interface ICamundaWorkflowClient
{
    Task<string?> StartRequestProcessAsync(RequestEntity request, CancellationToken cancellationToken);
}
