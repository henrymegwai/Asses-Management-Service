namespace CloudWorks.Data.Entities;

public class BookingSiteProfile : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
}