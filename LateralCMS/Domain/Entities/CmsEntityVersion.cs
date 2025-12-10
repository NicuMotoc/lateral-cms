namespace LateralCMS.Domain.Entities;

public class CmsEntityVersion
{
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
    public string Payload { get; set; } = default!;
    public bool IsUnpublished { get; set; }
}
