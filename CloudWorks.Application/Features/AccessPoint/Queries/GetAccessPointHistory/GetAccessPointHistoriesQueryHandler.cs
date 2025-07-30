using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;

public class GetAccessPointHistoriesQueryHandler(
    IRepository<AccessPointHistory> repository,
    IValidator<GetAccessPointHistoriesQuery> validator,
    ILogger<GetAccessPointHistoriesQueryHandler> logger)
    : IQueryHandler<GetAccessPointHistoriesQuery, Response<Page<AccessPointHistoryDto>>>
{
    public async Task<Response<Page<AccessPointHistoryDto>>> Handle(
        GetAccessPointHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for GetAccessPointHistoriesQuery: {Errors}", 
                validationResult.Errors);
            return new Response<Page<AccessPointHistoryDto>>(false, null!, 
                "Validation failed",  
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var accessPointHistoryDtos =  GetAccessPointHistoryResultAsync(request);
        return new Response<Page<AccessPointHistoryDto>>(
            true, accessPointHistoryDtos, 
            "Access point histories retrieved successfully.");
    }
    
    private Page<AccessPointHistoryDto> GetAccessPointHistoryResultAsync(GetAccessPointHistoriesQuery request)
    {
        // Filtering
        var query = repository.Query(
            x => (x.SiteId == request.SiteId) &&
                 x.Timestamp >= request.Start &&
                 x.Timestamp <= request.End,
            null, [x => x.AccessPoint!, x => x.Site!]).AsQueryable();
        // Sorting
        query = request.QueryParamsModel.SortBy switch
        {
            "Name" => request.QueryParamsModel.Desc ? query.OrderByDescending(s => s.AccessPoint!.Name) :
                query.OrderBy(s => s.AccessPoint!.Name),
            "SiteName" => request.QueryParamsModel.Desc ? query.OrderByDescending(s => s.Site!.Name) : 
                query.OrderBy(s => s.Site!.Name),
            "Timestamp" => request.QueryParamsModel.Desc ? query.OrderByDescending(s => s.Timestamp) : 
                query.OrderBy(s => s.Timestamp),
            _ => request.QueryParamsModel.Desc ? query.OrderByDescending(s => s.AccessPoint!.Name) : 
                query.OrderBy(s => s.AccessPoint!.Name)
        };

        var queryResult = query.Select(s => s.MapToDto());

        // Pagination
        return PageExtension<AccessPointHistoryDto>.ToPageListAsync(queryResult,
            request.QueryParamsModel.PageNumber,
            request.QueryParamsModel.PageSize);
    }
}