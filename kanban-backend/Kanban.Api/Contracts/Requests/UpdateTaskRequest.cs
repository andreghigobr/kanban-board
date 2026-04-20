namespace Kanban.Api.Contracts.Requests;

public record UpdateTaskRequest(
    Guid TaskId,
    string Title,
    string? Description,
    string? Status = null
);