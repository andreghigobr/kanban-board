using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Contracts.Requests;

public record CreateTaskRequest(
    string Title,
    string? Description,
    TaskStatus Status = TaskStatus.ToDo
);