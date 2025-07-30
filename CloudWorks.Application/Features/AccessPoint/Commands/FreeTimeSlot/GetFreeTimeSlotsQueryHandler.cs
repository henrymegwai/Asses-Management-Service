using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;

public class GetFreeTimeSlotsQueryHandler(IRepository<Data.Entities.Booking> bookingRepository,
    IValidator<GetFreeTimeSlotsQuery> validator,
    ILogger<GetFreeTimeSlotsQueryHandler> logger)
    : IQueryHandler<GetFreeTimeSlotsQuery, Response<List<TimeSlotDto>>>
{
    public async Task<Response<List<TimeSlotDto>>> Handle(GetFreeTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Query}: {Errors}", nameof(GetFreeTimeSlotsQuery),
                validationResult.Errors);
            return new Response<List<TimeSlotDto>>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        // Get all access point in the range for bookings
        var intervals = await GetBookedIntervalsAsync(request, cancellationToken);

        if (intervals!.Any())
        {
            // Find free slots
            var freeSlots = GetFreeSlots(intervals, request); 
            return new Response<List<TimeSlotDto>>(true, freeSlots, "Free time slots retrieved successfully.");
        }
        
        return  new Response<List<TimeSlotDto>>(true,
            [new TimeSlotDto { Start = request.From.ToCalDateTime(), End = request.To.ToCalDateTime() }], 
            "No bookings found, returning the entire range as a free slot.");
       
    }

    private async Task<IEnumerable<(CalDateTime Start, CalDateTime End)>> GetBookedIntervalsAsync(
        GetFreeTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var bookingResult 
            = await bookingRepository.GetAllAsync(b => b.AccessPoints.Any(ap => ap.Id == request.AccessPointId), 
                [], cancellationToken);
        
        if (bookingResult is { IsSuccess: true, Value: null })
            return Enumerable.Empty<(CalDateTime Start, CalDateTime End)>();
        
        var calFrom = request.From.ToCalDateTime();
        var calTo = request.To.ToCalDateTime();
        
        var result = bookingResult.Value!
            .SelectMany(b => b.Schedules
                .Where(s => s.Value.GetScheduleStartEnd().Start < calTo &&
                            s.Value.GetScheduleStartEnd().End > calFrom)
                .Select(s => new
                {
                    Start = s.Value.GetScheduleStartEnd().Start < calFrom
                        ? calFrom
                        : s.Value.GetScheduleStartEnd().Start,
                    End = s.Value.GetScheduleStartEnd().End > calTo
                        ? calTo
                        : s.Value.GetScheduleStartEnd().End
                }))
            .OrderBy(i => i.Start);

        return result.Select(i => (Start: i.Start!, End: i.End!));
    }
    
    private static List<TimeSlotDto> GetFreeSlots(
        IEnumerable<(CalDateTime Start, CalDateTime End)> intervals,
        GetFreeTimeSlotsQuery request)
    {
        var freeSlots = new List<TimeSlotDto>();
        var currentFrom = request.From.ToCalDateTime();
        var currentTo = request.To.ToCalDateTime();

        foreach (var interval in intervals)
        {
            if (interval.Start > currentFrom)
            {
                var slot = new TimeSlotDto { Start = currentFrom, End = interval.Start };
                if (slot.End.Subtract(slot.Start).Minutes >= request.DurationMinutes)
                    freeSlots.Add(slot);
            }
            currentFrom = interval.End > currentFrom ? interval.End : currentFrom;
        }

        if (currentFrom < currentTo)
        {
            var slot = new TimeSlotDto { Start = currentFrom, End = currentTo };
            if (slot.End.Subtract(slot.Start).Minutes >= request.DurationMinutes)
                freeSlots.Add(slot);
        }

        return freeSlots;
    }
}