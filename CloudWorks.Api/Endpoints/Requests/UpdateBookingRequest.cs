namespace CloudWorks.Api.Endpoints.Requests;

public class UpdateBookingRequest
{
    public string Name { get; set; } = null!;
    public Guid SiteId { get; set; }
}