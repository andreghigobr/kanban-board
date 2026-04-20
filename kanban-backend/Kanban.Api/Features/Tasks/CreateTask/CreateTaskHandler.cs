using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Contracts.Responses;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Infrastructure.Persistence;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Features.Tasks.CreateTask;

public class CreateTaskHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateTaskHandler> _logger;

    public CreateTaskHandler(AppDbContext context, ILogger<CreateTaskHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskResponse> Handle(CreateTaskRequest request)
    {
        _logger.LogInformation("Creating new task with title: {Title}", request.Title);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Task creation failed: Title is required");
            throw new TaskValidationException("Title is required");
        }

        // Convert string status to enum
        if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var taskStatus))
        {
            _logger.LogWarning("Task creation failed: Invalid status '{Status}'", request.Status);
            throw new TaskValidationException($"Invalid status '{request.Status}'. Valid values are: todo, inprogress, done");
        }

        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = taskStatus,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.CreatedAt,
            task.UpdatedAt);
    }
}
