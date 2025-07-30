using CloudWorks.Application.Common.Enumeration;

namespace CloudWorks.Application.Dtos;

public record AccessPointHistoryDto(
    Guid AccessPointId,
    string? AccessPointName,
    Guid SiteId,
    string? SiteName,
    DateTime Timestamp,
    string? AccessPointStatus,
    string Reason
);