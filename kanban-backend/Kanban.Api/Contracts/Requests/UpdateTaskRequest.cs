using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Contracts.Requests;

public record UpdateTaskRequest(
    Guid TaskId,
    string Title,
    string? Description,
    TaskStatus Status
);