using Kanban.Api.Contracts.Responses;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Api.Features.Tasks.GetTasks;

public class GetTasksHandler
{
    private readonly AppDbContext _context;

    public GetTasksHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskResponse>> Handle()
    {
        var tasks = await _context.Tasks
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TaskResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Status.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync();

        return tasks;
    }
}

