namespace Kanban.Api.Contracts.Requests;

public class GetTasksRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Status { get; set; }
}