using Kanban.Api.Common.Errors;
using Kanban.Api.Infrastructure.Persistence;

namespace Kanban.Api.Features.Tasks.DeleteTask;

public class DeleteTaskHandler
{
    private readonly AppDbContext _context;

    public DeleteTaskHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            throw new TaskNotFoundException(id);

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}
