using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;

public record GetAccessPointsBySiteIdQuery(Guid SiteId) : IQuery<Response<List<AccessPointDto>>>;