using CloudWorks.Application.Common.Enumeration;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Commands.OpenAccessPoint.Commands;

public class OpenAccessPointCommandHandler(
    IRepository<Data.Entities.AccessPoint> accessPointRepository,
    IRepository<Data.Entities.Booking> bookingRepository,
    IRepository<AccessPointHistory> accessPointHistoryRepository,
    IValidator<OpenAccessPointCommand> validator,
    ILogger<OpenAccessPointCommandHandler> logger)
    : ICommandHandler<OpenAccessPointCommand, Response<OpenAccessDto>>
{
    public async Task<Response<OpenAccessDto>> Handle(OpenAccessPointCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for OpenAccessPointCommand: {Errors}", validationResult.Errors);
            return new Response<OpenAccessDto>(false, null!, "Validation failed",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var accessPointResult = await accessPointRepository.GetByAsync(
            ap => ap.Id == request.AccessPointId && ap.SiteId == request.SiteId, cancellationToken);
        
        if (accessPointResult.IsFailed || accessPointResult.Value == null)
        {
            await LogAccessPointHistory(request, AccessPointStatus.Attempted, "Access point not found or does not belong to site", cancellationToken);
            return new Response<OpenAccessDto>(false, null!, "Access point not found or does not belong to site");
        }

        //Check for valid booking for this profile and access point at current time
        var now = CalDateTime.Now;
        var bookingResult = await bookingRepository.GetAllAsync(
            b => b.SiteId == request.SiteId
                 && b.AccessPoints.Any(ap => ap.Id == request.AccessPointId)
                 && b.SiteProfiles.Any(sp => sp.ProfileId == request.ProfileId)
                 && b.Schedules.Any(s => s.Value.GetScheduleStartEnd().Start <= now 
                                         && s.Value.GetScheduleStartEnd().End >= now),
            null, cancellationToken);

        if (bookingResult.IsFailed || bookingResult.Value.Count == 0)
        {
            await LogAccessPointHistory(request, AccessPointStatus.NotSuccessful, "No valid booking for this user at this time", cancellationToken);
            return new Response<OpenAccessDto>(false, null!, "No valid booking for this user at this time");
        }
        
        var result 
            = await LogAccessPointHistory(request, AccessPointStatus.Successful, "Access granted", cancellationToken);
        
        return result;
    }

    private async Task<Response<OpenAccessDto>> LogAccessPointHistory(
        OpenAccessPointCommand request, 
        AccessPointStatus accessPointStatus, 
        string reason, 
        CancellationToken cancellationToken)
    {
        var accessPointHistory = new AccessPointHistory
        {
            Id = Guid.NewGuid(),
            AccessPointId = request.AccessPointId,
            SiteId = request.SiteId, 
            ProfileId = request.ProfileId,
            Timestamp = DateTime.UtcNow,
            AccessPointStatus = accessPointStatus.ToString(), 
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            Reason = reason
        };
        
       var result =  await accessPointHistoryRepository.AddAsync(accessPointHistory, cancellationToken);

       if (result.IsSuccess)
           return new Response<OpenAccessDto>(true, result.Value.MapToOpenAccessDto(),
               "Access point history logged successfully");
       
       logger.LogError("Failed to log access point history: {Errors}", result.Errors);
        return new Response<OpenAccessDto>(false, null!, "Failed to log access point history",
            Result.Fail(result.Errors));
    }
}