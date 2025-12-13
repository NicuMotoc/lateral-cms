namespace LateralCMS.Application.DTOs;

public class CmsEventDto
{
    public string Type { get; set; } = default!;
    public int Id { get; set; }
    public int? Version { get; set; }
    public string? Payload { get; set; }
    public DateTime Timestamp { get; set; }
}
