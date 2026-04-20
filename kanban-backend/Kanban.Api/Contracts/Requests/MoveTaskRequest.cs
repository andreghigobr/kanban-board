namespace Kanban.Api.Contracts.Requests;

public record MoveTaskRequest(
    Guid TaskId,
    string Status
);