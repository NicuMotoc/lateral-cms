namespace LateralCMS.Domain.Entities;

public class CmsEntity
{
    public int Id { get; set; }
    public int LatestVersion { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDisabled { get; set; }
    public List<CmsEntityVersion> Versions { get; set; } = [];
}
