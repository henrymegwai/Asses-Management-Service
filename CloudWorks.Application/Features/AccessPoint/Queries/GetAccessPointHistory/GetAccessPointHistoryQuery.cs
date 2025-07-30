using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;

public record GetAccessPointHistoriesQuery(Guid? SiteId, QueryParamsModel QueryParamsModel, DateTime Start, DateTime End) 
    : IQuery<Response<Page<AccessPointHistoryDto>>>;