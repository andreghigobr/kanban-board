using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Infrastructure.Persistence;

namespace Kanban.Api.Features.Tasks.UpdateTask;

public class UpdateTaskHandler
{
    private readonly AppDbContext _context;

    public UpdateTaskHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTaskRequest request)
    {
        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
            throw new TaskNotFoundException(request.TaskId);

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new TaskValidationException("Title is required");

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
