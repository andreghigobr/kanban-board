using FluentAssertions;
using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Domain.Entities;
using Kanban.Api.Features.Tasks.UpdateTask;
using Microsoft.Extensions.Logging;
using Moq;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Tests.Features.Tasks.UpdateTask;

public class UpdateTaskHandlerTests : IDisposable
{
    private readonly TestAppDbContext _context;
    private readonly Mock<ILogger<UpdateTaskHandler>> _mockLogger;
    private readonly UpdateTaskHandler _handler;

    public UpdateTaskHandlerTests()
    {
        _context = new TestAppDbContext(Guid.NewGuid().ToString());
        _mockLogger = new Mock<ILogger<UpdateTaskHandler>>();
        _handler = new UpdateTaskHandler(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Old Title", Description = "Old Desc", Status = TaskStatus.ToDo, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();
        var request = new UpdateTaskRequest(taskId, "New Title", "New Description");

        // Act
        await _handler.Handle(request);

        // Assert
        var updatedTask = await _context.Tasks.FindAsync(taskId);
        updatedTask.Should().NotBeNull();
        updatedTask!.Title.Should().Be("New Title");
        updatedTask.Description.Should().Be("New Description");
        updatedTask.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest(taskId, "Title", "Description");

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Task with ID {taskId} not found");
    }

    [Fact]
    public async Task Handle_EmptyTitle_ThrowsTaskValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Old Title" };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();
        var request = new UpdateTaskRequest(taskId, "", "Description");

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskValidationException>()
            .WithMessage("Title is required");
    }

    [Fact]
    public async Task Handle_WithStatus_UpdatesTaskAndStatus()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Old Title", Status = TaskStatus.ToDo };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();
        var request = new UpdateTaskRequest(taskId, "New Title", "New Description", "done");

        // Act
        await _handler.Handle(request);

        // Assert
        var updatedTask = await _context.Tasks.FindAsync(taskId);
        updatedTask.Should().NotBeNull();
        updatedTask!.Title.Should().Be("New Title");
        updatedTask.Status.Should().Be(TaskStatus.Done);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ThrowsTaskValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem { Id = taskId, Title = "Old Title" };
        _context.Tasks.Add(existingTask);
        await _context.SaveChangesAsync();
        var request = new UpdateTaskRequest(taskId, "New Title", "Description", "invalid_status");

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskValidationException>()
            .WithMessage("*Invalid status*");
    }
}
