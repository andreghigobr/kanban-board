using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Features.Tasks.MoveTask;

public class MoveTaskHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<MoveTaskHandler> _logger;

    public MoveTaskHandler(AppDbContext context, ILogger<MoveTaskHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(MoveTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Moving task {TaskId} to status {Status}", request.TaskId, request.Status);

        // Convert string status to enum
        if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var taskStatus))
        {
            _logger.LogWarning("Task move failed: Invalid status '{Status}'", request.Status);
            throw new TaskValidationException($"Invalid status '{request.Status}'. Valid values are: todo, inprogress, done");
        }

        var task = await _context.Tasks.FindAsync(new object[] { request.TaskId }, cancellationToken);
        if (task == null)
        {
            _logger.LogWarning("Task move failed: Task with ID {TaskId} not found", request.TaskId);
            throw new TaskNotFoundException(request.TaskId);
        }

        task.Status = taskStatus;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task {TaskId} moved to status {Status} successfully", request.TaskId, taskStatus);
    }
}
