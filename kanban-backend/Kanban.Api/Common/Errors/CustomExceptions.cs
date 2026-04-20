namespace Kanban.Api.Common.Errors;

public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(Guid taskId)
        : base($"Task with ID {taskId} not found")
    {
    }
}

public class TaskValidationException : Exception
{
    public TaskValidationException(string message)
        : base(message)
    {
    }
}
