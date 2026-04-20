using FluentAssertions;
using Kanban.Api.Common.Errors;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Features.Tasks.DeleteTask;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kanban.Tests.Features.Tasks.DeleteTask;

public class DeleteTaskHandlerTests : IDisposable
{
    private readonly TestAppDbContext _context;
    private readonly Mock<ILogger<DeleteTaskHandler>> _mockLogger;
    private readonly DeleteTaskHandler _handler;

    public DeleteTaskHandlerTests()
    {
        _context = new TestAppDbContext(Guid.NewGuid().ToString());
        _mockLogger = new Mock<ILogger<DeleteTaskHandler>>();
        _handler = new DeleteTaskHandler(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ExistingTask_DeletesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Task to Delete" };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();

        // Act
        await _handler.Handle(taskId);

        // Assert
        var deletedTask = await _context.Tasks.FindAsync(taskId);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _handler.Handle(taskId);

        // Assert
        await act.Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Task with ID {taskId} not found");
    }
}
