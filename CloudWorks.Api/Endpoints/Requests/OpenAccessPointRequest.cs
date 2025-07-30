namespace CloudWorks.Api.Endpoints.Requests;

public class OpenAccessPointRequest
{
    public Guid ProfileId { get; set; }
    public Guid AccessPointId { get; set; }
    public Guid SiteId { get; set; }
}
