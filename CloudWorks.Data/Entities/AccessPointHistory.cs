namespace CloudWorks.Data.Entities;

public class AccessPointHistory: BaseEntity
{
    public Guid AccessPointId { get; set; }
    public AccessPoint? AccessPoint { get; set; }
    public string? AccessPointStatus { get; set; } = null!;
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = null!;
}