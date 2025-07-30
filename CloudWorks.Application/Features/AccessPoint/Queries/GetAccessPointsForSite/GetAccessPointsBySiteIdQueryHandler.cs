using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;

public class GetAccessPointsBySiteIdQueryHandler(
    IRepository<Data.Entities.AccessPoint> repository,
    IValidator<GetAccessPointsBySiteIdQuery> validator,
    ILogger<GetAccessPointsBySiteIdQueryHandler> logger)
    : IQueryHandler<GetAccessPointsBySiteIdQuery, Response<List<AccessPointDto>>>
{
    public async Task<Response<List<AccessPointDto>>> Handle(GetAccessPointsBySiteIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for GetAccessPointsBySiteIdQuery: {Errors}", 
                validationResult.Errors);
            return new Response<List<AccessPointDto>>(false, null!, 
                "Validation failed", 
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var result = await repository.GetAllAsync(ap => ap.SiteId == request.SiteId,[x=> x.Site!],
            cancellationToken);

        if(result.IsFailed)
        {
            logger.LogError("Failed to retrieve access points for site {SiteId}: {Errors}", 
                request.SiteId, result.Errors);
            return new Response<List<AccessPointDto>>(false, null!, 
                "Failed to retrieve access points.");
        }
        
        var accessPointDtos = result.Value.Select(ap => ap.MapToDto())
            .ToList();
        return new Response<List<AccessPointDto>>(true, accessPointDtos, "Access points retrieved successfully.");
    }
}