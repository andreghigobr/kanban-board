using Kanban.Api.Domain.Entities;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Api.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void SeedDatabase(AppDbContext context)
    {
        if (context.Tasks.Any())
            return; // Database already seeded

        var sampleTasks = new List<TaskItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Design UI mockups",
                Description = "Create mockups for the kanban board interface",
                Status = TaskStatus.InProgress,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Implement API endpoints",
                Description = "Build RESTful API for task management",
                Status = TaskStatus.InProgress,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Setup database",
                Description = "Configure Entity Framework and create migrations",
                Status = TaskStatus.Done,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Write unit tests",
                Description = "Add test coverage for API endpoints",
                Status = TaskStatus.ToDo,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Frontend integration",
                Description = "Connect React frontend with the API",
                Status = TaskStatus.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Tasks.AddRange(sampleTasks);
        context.SaveChanges();
    }
}