using Camunda.Worker;
using Microsoft.Extensions.Options;
using WorkflowMvp.Api.Models;

namespace WorkflowMvp.Api.Camunda;

public class CamundaWorkflowClient(ZeebeClient zeebeClient, IOptions<CamundaOptions> options, ILogger<CamundaWorkflowClient> logger) : ICamundaWorkflowClient
{
    private readonly CamundaOptions _options = options.Value;

    public async Task<string?> StartRequestProcessAsync(RequestEntity request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await zeebeClient
                .NewCreateProcessInstanceCommand()
                .BpmnProcessId(_options.ProcessId)
                .LatestVersion()
                .Variables(new
                {
                    requestId = request.Id,
                    title = request.Title,
                    description = request.Description,
                    createdBy = request.CreatedBy
                })
                .Send(cancellationToken);

            return response.ProcessInstanceKey.ToString();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to start Camunda process for request {RequestId}. Continuing for MVP.", request.Id);
            return null;
        }
    }
}
