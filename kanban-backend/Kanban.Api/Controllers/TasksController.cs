using Kanban.Api.Contracts.Requests;
using Kanban.Api.Contracts.Responses;
using Kanban.Api.Features.Tasks.CreateTask;
using Kanban.Api.Features.Tasks.DeleteTask;
using Kanban.Api.Features.Tasks.GetTasks;
using Kanban.Api.Features.Tasks.MoveTask;
using Kanban.Api.Features.Tasks.UpdateTask;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kanban.Api.Controllers;

[ApiController]
[Route("kanban/[controller]")]
public class TasksController : ControllerBase
{
    private readonly GetTasksHandler _getTasksHandler;
    private readonly CreateTaskHandler _createTaskHandler;
    private readonly UpdateTaskHandler _updateTaskHandler;
    private readonly DeleteTaskHandler _deleteTaskHandler;
    private readonly MoveTaskHandler _moveTaskHandler;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        GetTasksHandler getTasksHandler,
        CreateTaskHandler createTaskHandler,
        UpdateTaskHandler updateTaskHandler,
        DeleteTaskHandler deleteTaskHandler,
        MoveTaskHandler moveTaskHandler,
        ILogger<TasksController> logger)
    {
        _getTasksHandler = getTasksHandler;
        _createTaskHandler = createTaskHandler;
        _updateTaskHandler = updateTaskHandler;
        _deleteTaskHandler = deleteTaskHandler;
        _moveTaskHandler = moveTaskHandler;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll()
    {
        _logger.LogInformation("HTTP GET /kanban/tasks - Retrieving all tasks");

        var tasks = await _getTasksHandler.Handle();

        _logger.LogInformation("HTTP GET /kanban/tasks - Returned {TaskCount} tasks", tasks.Count());

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request)
    {
        _logger.LogInformation("HTTP POST /kanban/tasks - Creating new task with title: {Title}", request.Title);

        var response = await _createTaskHandler.Handle(request);

        _logger.LogInformation("HTTP POST /kanban/tasks - Task created with ID: {TaskId}", response.TaskId);

        return CreatedAtAction(nameof(GetAll), new { id = response.TaskId }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTaskRequest request)
    {
        _logger.LogInformation("HTTP PUT /kanban/tasks - Updating task with ID: {TaskId}", request.TaskId);

        await _updateTaskHandler.Handle(request);

        _logger.LogInformation("HTTP PUT /kanban/tasks - Task with ID {TaskId} updated successfully", request.TaskId);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("HTTP DELETE /kanban/tasks/{TaskId} - Deleting task", id);

        await _deleteTaskHandler.Handle(id);

        _logger.LogInformation("HTTP DELETE /kanban/tasks/{TaskId} - Task deleted successfully", id);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MoveTask(Guid id, MoveTaskRequest request)
    {
        _logger.LogInformation("HTTP PATCH /kanban/tasks/{TaskId} - Moving task to status {Status}", id, request.Status);

        var moveRequest = request with { TaskId = id };
        await _moveTaskHandler.Handle(moveRequest);

        _logger.LogInformation("HTTP PATCH /kanban/tasks/{TaskId} - Task moved to status {Status} successfully", id, request.Status);

        return NoContent();
    }
}