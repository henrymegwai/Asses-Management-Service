using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;

namespace CloudWorks.Application.Features.Site.Queries.GetSites;

public class GetSitesQueryHandler(IRepository<Data.Entities.Site> siteRepository)
    : IQueryHandler<GetSitesQuery, Response<Page<SiteDto>>>
{
    public async Task<Response<Page<SiteDto>>> Handle(GetSitesQuery request, CancellationToken cancellationToken)
    {
        var result = GetSitesResultAsync(request);
        
        return new Response<Page<SiteDto>>(true, result, "Sites retrieved successfully.");
    }
    
    private Page<SiteDto> GetSitesResultAsync(GetSitesQuery request)
    {
        // Filtering
        var query = string.IsNullOrEmpty(request.QueryParams.Name)
            ? siteRepository.Query(null, null, [])
            : siteRepository.Query(x => x.Name == request.QueryParams.Name, null, []);

        // Sorting
        query = request.QueryParams.SortBy switch
        {
            "Name" => request.QueryParams.Desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            _ => request.QueryParams.Desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name)
        };

        var queryResult = query.Select(s => s.MapToDto());

        // Pagination
        return PageExtension<SiteDto>.ToPageListAsync(queryResult,
            request.QueryParams.PageNumber,
            request.QueryParams.PageSize);
    }
}