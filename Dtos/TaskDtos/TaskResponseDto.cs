namespace Dtos;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }
    public string CurrentVersionStatus { get; set; }
    public int DocumentCount { get; set; }
    public string FirstUpdaterName { get; set; }
    public string LastUpdaterName { get; set; }
}