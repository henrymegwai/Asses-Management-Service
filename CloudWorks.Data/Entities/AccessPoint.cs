namespace CloudWorks.Data.Entities;

public sealed class AccessPoint : BaseEntity
{
    public string Name { get; set; } = null!;

    public Guid SiteId { get; set; }

    public Site? Site { get; set; }
}