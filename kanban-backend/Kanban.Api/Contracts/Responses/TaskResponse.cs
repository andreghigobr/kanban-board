namespace Kanban.Api.Contracts.Responses;

public record TaskResponse(
    Guid TaskId,
    string Title,
    string? Description,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);