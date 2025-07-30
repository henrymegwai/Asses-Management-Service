namespace CloudWorks.Api.Endpoints.Requests;

public class UpdateAccessPointRequest
{
    public string Name { get; set; } = null!;
    public Guid SiteId { get; set; }
}