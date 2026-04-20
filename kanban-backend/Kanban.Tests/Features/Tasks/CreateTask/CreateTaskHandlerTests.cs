using FluentAssertions;
using Kanban.Api.Common.Errors;
using Kanban.Api.Contracts.Requests;
using Kanban.Api.Features.Tasks.CreateTask;
using Microsoft.Extensions.Logging;
using Moq;
using TaskStatus = Kanban.Api.Domain.Enums.TaskStatus;

namespace Kanban.Tests.Features.Tasks.CreateTask;

public class CreateTaskHandlerTests : IDisposable
{
    private readonly TestAppDbContext _context;
    private readonly Mock<ILogger<CreateTaskHandler>> _mockLogger;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _context = new TestAppDbContext(Guid.NewGuid().ToString());
        _mockLogger = new Mock<ILogger<CreateTaskHandler>>();
        _handler = new CreateTaskHandler(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesTaskAndReturnsResponse()
    {
        // Arrange
        var request = new CreateTaskRequest("Test Task", "Description", TaskStatus.ToDo);

        // Act
        var result = await _handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Task");
        result.Description.Should().Be("Description");
        result.Status.Should().Be("ToDo");

        var taskInDb = await _context.Tasks.FindAsync(result.TaskId);
        taskInDb.Should().NotBeNull();
        taskInDb!.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task Handle_EmptyTitle_ThrowsTaskValidationException()
    {
        // Arrange
        var request = new CreateTaskRequest("", "Description");

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskValidationException>()
            .WithMessage("Title is required");
    }

    [Fact]
    public async Task Handle_WhitespaceTitle_ThrowsTaskValidationException()
    {
        // Arrange
        var request = new CreateTaskRequest("   ", "Description");

        // Act
        Func<Task> act = async () => await _handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<TaskValidationException>()
            .WithMessage("Title is required");
    }
}
