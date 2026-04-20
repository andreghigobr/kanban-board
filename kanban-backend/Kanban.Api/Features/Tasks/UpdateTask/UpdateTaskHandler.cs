using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Features.Tasks.UpdateTask;

public class UpdateTaskHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateTaskHandler> _logger;

    public UpdateTaskHandler(AppDbContext context, ILogger<UpdateTaskHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", request.TaskId);

        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
        {
            _logger.LogWarning("Task update failed: Task with ID {TaskId} not found", request.TaskId);
            throw new TaskNotFoundException(request.TaskId);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Task update failed: Title is required for task ID {TaskId}", request.TaskId);
            throw new TaskValidationException("Title is required");
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var taskStatus))
            {
                _logger.LogWarning("Task update failed: Invalid status '{Status}' for task ID {TaskId}", request.Status, request.TaskId);
                throw new TaskValidationException($"Invalid status '{request.Status}'. Valid values are: todo, inprogress, done");
            }
            task.Status = taskStatus;
        }

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Task with ID {TaskId} updated successfully", request.TaskId);
    }
}
