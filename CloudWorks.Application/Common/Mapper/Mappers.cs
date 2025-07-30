using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Data.Entities;

namespace CloudWorks.Application.Common.Mapper;

public static class Mappers
{
    public static SiteDto MapToDto(this Site site)
    {
        return new SiteDto(
            Id: site.Id,
            Name: site.Name, 
            Users: site.Profiles != null! ? site.Profiles
                .Select(sp => sp.Profile!)
                .Where(p => p != null!)
                .Select(p => p!.MapToDto())
                .ToList() : null!);
    }

    private static ProfileDto MapToDto(this Profile profile)
    {
        return new ProfileDto(
            Id: profile.Id,
            Email: profile.Email,
            IdentityId: profile.IdentityId);
    }
    
    public static SiteProfileDto MapToDto(this SiteProfile siteProfile)
    {
        return new SiteProfileDto(
            ProfileId: siteProfile.ProfileId,
            Profile: siteProfile.Profile?.MapToDto(),
            SiteId: siteProfile.SiteId,
            Site: siteProfile.Site != null ? new SiteDto(
                Id: siteProfile.Site!.Id,
                Name: siteProfile.Site.Name, 
                Users: siteProfile.Site.Profiles
                    .Select(sp => sp.Profile)
                    .Where(p => p != null)
                    .Select(p => p!.MapToDto())
                    .ToList()) : null!);
    }
    
    public static BookingDto MapToDto(this Booking booking)
    {
        return new BookingDto(
            Name: booking.Name,
            UserEmails: booking.SiteProfiles != null! ? booking.SiteProfiles.Select(x => x.Profile!.Email).ToList() : null!,
            AccessPoints: booking.AccessPoints != null! ? 
                booking.AccessPoints.Select(x => x.Id).ToList() : null!,
            Schedules: booking.Schedules != null!
                ? booking.Schedules.Select(s =>
                {
                    var schedule = s.Value.GetScheduleStartEnd();
                    return new ScheduleModel
                    {
                        Start = schedule.Start!.Value,
                        End = schedule.End!.Value,
                    };
                }).ToList()
                : null!
        );
    }
    
    public static AccessPointDto MapToDto(this AccessPoint accessPoint)
    {
        return new AccessPointDto(
            Id: accessPoint.Id,
            Name: accessPoint.Name,
            SiteId: accessPoint.SiteId,
            Site: accessPoint.Site != null
                ? new SiteDto(
                    Id: accessPoint.Site.Id,
                    Name: accessPoint.Site.Name,
                    Users: accessPoint.Site.Profiles
                        .Select(sp => sp.Profile)
                        .Where(p => p != null)
                        .Select(p => p!.MapToDto())
                        .ToList())
                : null!);
    }
    
    public static AccessPointHistoryDto MapToDto(this AccessPointHistory accessPointHistory)
    {
        return new AccessPointHistoryDto(
            AccessPointId: accessPointHistory.AccessPointId,
            AccessPointName: accessPointHistory.AccessPoint!.Name,
            SiteId: accessPointHistory.SiteId,
            SiteName: accessPointHistory.Site!.Name,
            Timestamp: accessPointHistory.Timestamp,
            AccessPointStatus: accessPointHistory.AccessPointStatus!,
            Reason: accessPointHistory.Reason);
    }
    
    public static OpenAccessDto MapToOpenAccessDto(this AccessPointHistory accessPointHistory)
    {
        return new OpenAccessDto
        {
            Id = accessPointHistory.Id,
            Name = accessPointHistory.AccessPoint!.Name,
            SiteId = accessPointHistory.SiteId,
            Site = accessPointHistory.Site != null ? new SiteDto(
                Id: accessPointHistory.Site!.Id,
                Name: accessPointHistory.Site.Name,
                Users: accessPointHistory.Site.Profiles
                    .Select(sp => sp.Profile)
                    .Where(p => p != null)
                    .Select(p => p!.MapToDto())
                    .ToList()) : null!,
            AccessPointStatus = accessPointHistory.AccessPointStatus!,
            OpenedAt = accessPointHistory.Timestamp,
            ClosedAt = null!
        };
    }
}