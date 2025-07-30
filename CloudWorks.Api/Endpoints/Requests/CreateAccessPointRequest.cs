namespace CloudWorks.Api.Endpoints.Requests;

public class CreateAccessPointRequest
{
    public string Name { get; set; } = null!;

    public Guid SiteId { get; set; }
}