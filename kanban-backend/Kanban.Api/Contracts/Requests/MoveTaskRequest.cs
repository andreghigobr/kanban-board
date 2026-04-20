using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Contracts.Requests;

public record MoveTaskRequest(
    Guid TaskId,
    TaskStatus Status
);