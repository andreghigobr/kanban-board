using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Infrastructure.Persistence;

namespace Kanban.Api.Features.Tasks.MoveTask;

public class MoveTaskHandler
{
    private readonly AppDbContext _context;

    public MoveTaskHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MoveTaskRequest request)
    {
        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
            throw new TaskNotFoundException(request.TaskId);

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
