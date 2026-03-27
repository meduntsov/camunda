namespace WorkflowMvp.Api.Models;

public class RequestCommentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RequestId { get; set; }
    public RequestEntity? Request { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
