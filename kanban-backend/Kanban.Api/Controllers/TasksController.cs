using Kanban.Api.Contracts.Requests;
using Kanban.Api.Contracts.Responses;
using Kanban.Api.Features.Tasks.CreateTask;
using Kanban.Api.Features.Tasks.DeleteTask;
using Kanban.Api.Features.Tasks.GetTasks;
using Kanban.Api.Features.Tasks.MoveTask;
using Kanban.Api.Features.Tasks.UpdateTask;
using Microsoft.AspNetCore.Mvc;

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

    public TasksController(
        GetTasksHandler getTasksHandler,
        CreateTaskHandler createTaskHandler,
        UpdateTaskHandler updateTaskHandler,
        DeleteTaskHandler deleteTaskHandler,
        MoveTaskHandler moveTaskHandler)
    {
        _getTasksHandler = getTasksHandler;
        _createTaskHandler = createTaskHandler;
        _updateTaskHandler = updateTaskHandler;
        _deleteTaskHandler = deleteTaskHandler;
        _moveTaskHandler = moveTaskHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll()
    {
        var tasks = await _getTasksHandler.Handle();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request)
    {
        var response = await _createTaskHandler.Handle(request);
        return CreatedAtAction(nameof(GetAll), new { id = response.TaskId }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTaskRequest request)
    {
        await _updateTaskHandler.Handle(request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteTaskHandler.Handle(id);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MoveTask(Guid id, MoveTaskRequest request)
    {
        var moveRequest = request with { TaskId = id };
        await _moveTaskHandler.Handle(moveRequest);
        return NoContent();
    }
}