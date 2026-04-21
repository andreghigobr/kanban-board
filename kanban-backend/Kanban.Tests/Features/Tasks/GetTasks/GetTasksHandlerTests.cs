using FluentAssertions;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Features.Tasks.GetTasks;
using Microsoft.Extensions.Logging;
using Moq;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Tests.Features.Tasks.GetTasks;

public class GetTasksHandlerTests : IDisposable
{
    private readonly TestAppDbContext _context;
    private readonly Mock<ILogger<GetTasksHandler>> _mockLogger;
    private readonly GetTasksHandler _handler;

    public GetTasksHandlerTests()
    {
        _context = new TestAppDbContext(Guid.NewGuid().ToString());
        _mockLogger = new Mock<ILogger<GetTasksHandler>>();
        _handler = new GetTasksHandler(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ReturnsAllTasksOrderedByCreatedAtDescending()
    {
        // Arrange
        var task1 = new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Status = TaskStatus.ToDo, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow };
        var task2 = new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Status = TaskStatus.InProgress, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetTasksRequest());

        // Assert
        result.Tasks.Should().HaveCount(2);
        result.Tasks.First().Title.Should().Be("Task 2"); // Ordered by CreatedAt descending
        result.Tasks.Last().Title.Should().Be("Task 1");
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await _handler.Handle(new GetTasksRequest());

        // Assert
        result.Tasks.Should().BeEmpty();
    }
}
