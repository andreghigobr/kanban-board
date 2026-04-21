using Kanban.Api.Common.Errors;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Kanban.Api.Features.Tasks.DeleteTask;

public class DeleteTaskHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteTaskHandler> _logger;

    public DeleteTaskHandler(AppDbContext context, ILogger<DeleteTaskHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);

        var task = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task == null)
        {
            _logger.LogWarning("Task deletion failed: Task with ID {TaskId} not found", id);
            throw new TaskNotFoundException(id);
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task with ID {TaskId} deleted successfully", id);
    }
}
