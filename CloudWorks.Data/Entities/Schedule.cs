namespace CloudWorks.Data.Entities;
public sealed class Schedule : BaseEntity
{
    public string Value { get; set; } = null!;//https://www.rfc-editor.org/rfc/rfc7529

    public Guid SiteId { get; set; }

    public Site? Site { get; set; }

    public Guid BookingId { get; set; }

    public Booking? Booking { get; set; }
}
