namespace CloudWorks.Data.Entities;
public sealed class SiteProfile : BaseEntity
{
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
}