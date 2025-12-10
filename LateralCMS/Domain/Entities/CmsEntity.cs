namespace LateralCMS.Domain.Entities;

public class CmsEntity
{
    public string Id { get; set; } = default!;
    public int LatestVersion { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDisabledByAdmin { get; set; }
    public List<CmsEntityVersion> Versions { get; set; } = new();
}

public class CmsEntityVersion
{
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
    public string Payload { get; set; } = default!; // JSON serialized
    public bool IsUnpublished { get; set; }
}
