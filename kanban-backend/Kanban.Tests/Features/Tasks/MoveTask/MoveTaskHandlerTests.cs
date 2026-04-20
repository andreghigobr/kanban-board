using FluentAssertions;
using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Features.Tasks.MoveTask;
using Microsoft.Extensions.Logging;
using Moq;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Tests.Features.Tasks.MoveTask;

public class MoveTaskHandlerTests : IDisposable
{
    private readonly TestAppDbContext _context;
    private readonly Mock<ILogger<MoveTaskHandler>> _mockLogger;
    private readonly MoveTaskHandler _handler;

    public MoveTaskHandlerTests()
    {
        _context = new TestAppDbContext(Guid.NewGuid().ToString());
        _mockLogger = new Mock<ILogger<MoveTaskHandler>>();
        _handler = new MoveTaskHandler(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ExistingTask_UpdatesStatusAndUpdatedAt()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Task", Status = TaskStatus.ToDo, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();
        var request = new MoveTaskRequest(taskId, TaskStatus.InProgress);

        // Act
        await _handler.Handle(request);

        // Assert
        var updatedTask = await _context.Tasks.FindAsync(taskId);
        updatedTask.Should().NotBeNull();
        updatedTask!.Status.Should().Be(TaskStatus.InProgress);
        updatedTask.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new MoveTaskRequest(taskId, TaskStatus.Done);

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Task with ID {taskId} not found");
    }
}
