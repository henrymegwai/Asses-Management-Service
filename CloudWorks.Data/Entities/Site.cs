namespace CloudWorks.Data.Entities;

public sealed class Site : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<SiteProfile> Profiles { get; set; } = [];
}