namespace LateralCMS.Application.DTOs;

public class CmsEventDto
{
    public string Type { get; set; } = default!; // publish, delete, unPublish
    public string Id { get; set; } = default!;
    public int? Version { get; set; }
    public string? Payload { get; set; } // JSON
    public DateTime Timestamp { get; set; }
}
