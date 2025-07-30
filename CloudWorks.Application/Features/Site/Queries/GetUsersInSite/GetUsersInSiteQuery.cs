using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Site.Queries.GetUsersInSite;

public record GetUsersInSiteQuery(Guid SiteId) : IQuery<Response<SiteDto>>;