using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

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

    public async Task Handle(MoveTaskRequest request)
    {
        _logger.LogInformation("Moving task {TaskId} to status {Status}", request.TaskId, request.Status);

        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
        {
            _logger.LogWarning("Task move failed: Task with ID {TaskId} not found", request.TaskId);
            throw new TaskNotFoundException(request.TaskId);
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} moved to status {Status} successfully", request.TaskId, request.Status);
    }
}
