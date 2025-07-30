namespace CloudWorks.Application.Dtos;

public record SiteProfileDto(
    Guid ProfileId, 
    ProfileDto? Profile,
    Guid SiteId, 
    SiteDto? Site);