using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Site.Queries.GetSites;

public record GetSitesQuery(QueryParamsModel QueryParams) : IQuery<Response<Page<SiteDto>>>;