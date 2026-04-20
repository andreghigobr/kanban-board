using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Contracts.Responses;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Infrastructure.Persistence;

namespace Kanban.Api.Features.Tasks.CreateTask;

public class CreateTaskHandler
{
    private readonly AppDbContext _context;

    public CreateTaskHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse> Handle(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new TaskValidationException("Title is required");

        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.CreatedAt,
            task.UpdatedAt);
    }
}
