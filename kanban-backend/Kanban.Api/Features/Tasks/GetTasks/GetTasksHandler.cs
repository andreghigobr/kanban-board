using Kanban.Api.Contracts.Responses;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kanban.Api.Features.Tasks.GetTasks;

public class GetTasksHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetTasksHandler> _logger;

    public GetTasksHandler(AppDbContext context, ILogger<GetTasksHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskResponse>> Handle()
    {
        _logger.LogInformation("Retrieving all tasks");

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

        _logger.LogInformation("Retrieved {TaskCount} tasks", tasks.Count);

        return tasks;
    }
}
