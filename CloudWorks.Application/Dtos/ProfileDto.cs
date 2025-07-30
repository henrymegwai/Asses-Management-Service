namespace CloudWorks.Application.Dtos;

public record ProfileDto(
    Guid Id, 
    string Email, 
    Guid? IdentityId);