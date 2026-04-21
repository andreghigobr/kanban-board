namespace Kanban.Api.Contracts.Requests;

public class GetTasksRequest
{
    public required string Status { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}