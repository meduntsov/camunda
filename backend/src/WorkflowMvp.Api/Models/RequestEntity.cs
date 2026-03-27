namespace WorkflowMvp.Api.Models;

public class RequestEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Draft;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CamundaProcessInstanceKey { get; set; }
    public ICollection<RequestCommentEntity> Comments { get; set; } = new List<RequestCommentEntity>();
}
