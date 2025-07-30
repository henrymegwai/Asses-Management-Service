using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Site.Queries.GetUsersInSite;

public class GetUsersInSiteQueryHandler(IRepository<Data.Entities.Site> siteRepository,
    IValidator<GetUsersInSiteQuery> validator,
    ILogger<GetUsersInSiteQueryHandler> logger)
    : IQueryHandler<GetUsersInSiteQuery, Response<SiteDto>>
{
    public async Task<Response<SiteDto>> Handle(GetUsersInSiteQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Query}: {Errors}", nameof(GetUsersInSiteQuery),
                validationResult.Errors);
            return new Response<SiteDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var siteResult = await siteRepository.GetByAsync(x => x.Id == request.SiteId, cancellationToken);
        
        if (siteResult is { IsSuccess: true, Value: null })
        {
            return new Response<SiteDto>(false, null!, "Site not found.");
        }

        if(siteResult.IsFailed)
        {
            logger.LogError("Failed to retrieve site: {Errors}", siteResult.Errors);
            return new Response<SiteDto>(false, null!, "Failed to retrieve site.");
        }

        var usersInSite = GetUsersInSiteProfiles(siteResult.Value!);

        if (usersInSite.Count == 0)
        {
            return new Response<SiteDto>(true, null!, "No users found in the site.");
        }

        var siteDto = new SiteDto(siteResult.Value!.Id, siteResult.Value.Name, usersInSite);

        return new Response<SiteDto>(true, siteDto, "Users retrieved successfully.");
    }
    
    private List<ProfileDto> GetUsersInSiteProfiles(Data.Entities.Site site)
    {
        return site.Profiles
            .Select(sp => sp.Profile)
            .Where(p => p != null)
            .Select(p => new ProfileDto(p!.Id, p.Email, p.IdentityId))
            .ToList();
    }
}