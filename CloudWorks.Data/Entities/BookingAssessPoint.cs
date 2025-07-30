namespace CloudWorks.Data.Entities;

public class BookingAssessPoint : BaseEntity
{
    public Guid AccessPointsId { get; set; }
    public Guid BookingId { get; set; }
    public AccessPoint? AccessPoints { get; set; }
    public Booking? Booking { get; set; }
}