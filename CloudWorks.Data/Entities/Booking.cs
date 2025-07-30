namespace CloudWorks.Data.Entities;
public sealed class Booking : BaseEntity
{
    public string Name { get; set; } = null!;

    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public ICollection<SiteProfile> SiteProfiles { get; set; } = [];
    public ICollection<Schedule> Schedules { get; set; } = [];
    public ICollection<AccessPoint> AccessPoints { get; set; } = [];
}
