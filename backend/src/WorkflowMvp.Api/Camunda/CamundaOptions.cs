namespace WorkflowMvp.Api.Camunda;

public class CamundaOptions
{
    public const string SectionName = "Camunda";
    public string GatewayAddress { get; set; } = "zeebe:26500";
    public string ProcessId { get; set; } = "request_approval_process";
}
