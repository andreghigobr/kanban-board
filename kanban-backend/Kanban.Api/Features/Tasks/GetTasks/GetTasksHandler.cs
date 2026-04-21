using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Contracts.Responses;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

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

    public async Task<GetTasksResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving tasks with filters - Status: {Status}, Page: {Page}, PageSize: {PageSize}",
            request.Status, request.Page, request.PageSize);

        // Validate status
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            _logger.LogWarning("Status is required");
            throw new TaskValidationException("Status is required");
        }

        if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var taskStatus))
        {
            _logger.LogWarning("Invalid status filter: {Status}", request.Status);
            throw new TaskValidationException($"Invalid status '{request.Status}'. Valid values are: todo, inprogress, done");
        }

        // Set defaults
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 50; // Default page size

        // Validate parameters
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 50; // Max page size
        }

        // Build query
        var query = _context.Tasks.AsQueryable();

        // Apply status filter (now mandatory)
        query = query.Where(x => x.Status == taskStatus);
        _logger.LogInformation("Applied status filter: {Status}", taskStatus);

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering and pagination
        var tasks = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TaskResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Status.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {TaskCount} tasks (page {Page} of {TotalPages}, total: {TotalCount})",
            tasks.Count, page, (int)Math.Ceiling((double)totalCount / pageSize), totalCount);

        return new GetTasksResponse
        {
            Tasks = tasks,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
