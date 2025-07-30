namespace CloudWorks.Application.Dtos;

public record SiteDto(
    Guid Id,
    string Name,
    List<ProfileDto> Users);