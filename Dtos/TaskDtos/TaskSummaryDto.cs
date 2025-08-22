namespace Dtos;

public class TaskSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime CreateDate { get; set; }
    public string Status { get; set; }
    public int DocumentCount { get; set; }
}