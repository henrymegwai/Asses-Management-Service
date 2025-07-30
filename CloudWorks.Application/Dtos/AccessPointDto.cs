namespace CloudWorks.Application.Dtos;

public record AccessPointDto(
    Guid Id,
    string Name,
    Guid SiteId,
    SiteDto Site
);