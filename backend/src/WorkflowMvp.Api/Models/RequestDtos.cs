namespace WorkflowMvp.Api.Models;

public record CreateRequestDto(string Title, string Description);
public record ActionRequestDto(string? Comment);

public record RequestListItemDto(
    Guid Id,
    string Title,
    string Description,
    string CreatedBy,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record RequestDetailDto(
    Guid Id,
    string Title,
    string Description,
    string CreatedBy,
    string Status,
    string? CamundaProcessInstanceKey,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<RequestCommentDto> Comments,
    IReadOnlyCollection<AuditLogDto> AuditLog);

public record RequestCommentDto(string Author, string Comment, DateTimeOffset CreatedAt);
public record AuditLogDto(string Action, string PerformedBy, string Details, DateTimeOffset CreatedAt);

public record RequestSummaryDto(Guid RequestId, string Status, string Summary);
