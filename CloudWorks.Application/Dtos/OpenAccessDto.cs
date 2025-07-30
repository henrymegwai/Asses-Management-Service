namespace CloudWorks.Application.Dtos;

public class OpenAccessDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid SiteId { get; set; }
    public SiteDto Site { get; set; } = null!;
    public string AccessPointStatus { get; set; } = null!;
    public DateTime OpenedAt { get; set; } 
    public DateTime? ClosedAt { get; set; } 
}