using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandler(
    ILogger<CreateBookingCommandHandler> logger,
    IValidator<CreateBookingCommand> validator,
    IRepository<Data.Entities.Booking> bookingRepository,
    IRepository<Schedule> scheduleRepository,
    IRepository<BookingAssessPoint> bookingAccessPointRepository,
    IRepository<BookingSiteProfile> bookingSiteProfileRepository,
    IRepository<Data.Entities.AccessPoint> accessPointRepository,
    IRepository<Profile> profileRepository)
    : ICommandHandler<CreateBookingCommand,  Response<BookingDto>>
{
    public async Task<Response<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(CreateBookingCommand),
                validationResult.Errors);
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(CreateBookingCommand), validationResult.Errors);
            return new Response<BookingDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var scheduleExistsResponse = await CheckIfScheduleExistsAsync(request, cancellationToken);
        if (scheduleExistsResponse != null)
        {
            return scheduleExistsResponse;
        }
        
        var bookingResult  = await CreateAndAddBookingAsync(request, cancellationToken);

        if (bookingResult.IsSuccess)
        {
            return new Response<BookingDto>(true, bookingResult.Value.MapToDto(), 
                "Booking created successfully.");
        }

        logger.LogError("Failed to create booking: {Errors}", bookingResult.Errors);
        return new Response<BookingDto>(false, null!, "Failed to create booking.");
    }
    
    private async Task<Response<BookingDto>?> CheckIfScheduleExistsAsync(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var existingSchedules 
            = await scheduleRepository.GetAllAsync(x => x.SiteId == request.SiteId, [], cancellationToken);

        var schedule = DoesScheduleExist(existingSchedules.Value, request.Schedules);

        if (!schedule.Exist) return null;
        
        logger.LogWarning("Schedule already exists for booking: {Start} - {End}", schedule.scheduleModel.Start,
            schedule.scheduleModel.End);
        return new Response<BookingDto>(false, null!,
            $"Schedule already exists for booking: Start - {schedule.scheduleModel.Start} " +
            $"and End - {schedule.scheduleModel.End}");

    }
    
    private async Task<Result<Data.Entities.Booking>> CreateAndAddBookingAsync(
        CreateBookingCommand request,
        CancellationToken cancellationToken)
    {
        // create booking
        var bookingResult = await bookingRepository.AddAsync(new Data.Entities.Booking()
        {
            Id = Guid.NewGuid(),
            SiteId = request.SiteId,
            Name = request.Name,
        }, cancellationToken);
        
        if(bookingResult.IsFailed)
        {
            logger.LogError("Failed to create booking: {Errors}", bookingResult.Errors);
            return Result.Fail(bookingResult.Errors);
        }
        var booking = bookingResult.Value;
        
        // create schedule for booking
        var schedules = CreateSchedules(request.Schedules, request.SiteId, booking.Id);
        await scheduleRepository.AddRangeAsync(schedules, cancellationToken);
        
        // create booking access points
        await AddBookingAccessPointsAsync(request, booking, cancellationToken);

        if (request.UsersEmails.Count <= 0) return bookingResult;
        
        // create booking site profiles
        await AddBookingSiteProfilesAsync(request, booking, cancellationToken);

        return bookingResult;
    }
    
    private static List<Schedule> CreateSchedules(
        IEnumerable<ScheduleModel> scheduleModels, 
        Guid siteId, 
        Guid bookingId)
    {
        var serializer = new CalendarSerializer();
        var schedules = new List<Schedule>();

        foreach (var sch in scheduleModels)
        {
            var e = new CalendarEvent
            {
                Start = new CalDateTime(sch.Start),
                End = new CalDateTime(sch.End)
            };

            var calendar = new Ical.Net.Calendar();
            calendar.Events.Add(e);

            schedules.Add(new Schedule
            {
                Id = Guid.NewGuid(),
                SiteId = siteId,
                Value = serializer.SerializeToString(calendar)!,
                BookingId = bookingId,
                CreatedAt = DateTime.Now, 
                CreatedBy = Guid.NewGuid()
            });
        }
        
        return schedules;
    }
    
    private async Task AddBookingAccessPointsAsync(
        CreateBookingCommand request, 
        Data.Entities.Booking booking, 
        CancellationToken cancellationToken)
    {
        var accessPoints = await accessPointRepository
            .GetAllAsync(x => x.SiteId == request.SiteId && request.AccessPoints.Contains(x.Id), [],
                cancellationToken);

        if (accessPoints.Value.Count > 0)
        {
            await bookingAccessPointRepository.AddRangeAsync(
                accessPoints.Value.Select(ap => new BookingAssessPoint
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id, 
                    AccessPointsId = ap.Id,
                    CreatedAt = DateTime.Now, 
                    CreatedBy = Guid.NewGuid()
                }).ToList(), cancellationToken);
        }
    }
    
    private async Task AddBookingSiteProfilesAsync(
        CreateBookingCommand request,
        Data.Entities.Booking booking,
        CancellationToken cancellationToken)
    {
        var profilesResult = await profileRepository.GetAllAsync(
            p => request.UsersEmails.Contains(p.Email), [], cancellationToken);

        if (profilesResult.Value.Count > 0)
        {
            await bookingSiteProfileRepository.AddRangeAsync(
                profilesResult.Value.Select(profile => new BookingSiteProfile
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    ProfileId = profile.Id, 
                    CreatedAt = DateTime.Now, 
                    CreatedBy = Guid.NewGuid()
                }).ToList(), cancellationToken);
        }
    }
    
    private static (ScheduleModel scheduleModel, bool Exist) DoesScheduleExist(
        IEnumerable<Schedule> schedules, 
        List<ScheduleModel> scheduleModels)
    {
        foreach (var existingSchedule in schedules)
        {
            var (existingStart, existingEnd) = existingSchedule.Value.GetScheduleStartEnd();

            var scheduleSet = new HashSet<(DateTime Start, DateTime End)>(
                scheduleModels.Select(s => (s.Start, s.End))
            );

            var isScheduleExist = scheduleSet.Any(s => s.Start <= existingEnd!.Value && s.End >= existingStart!.Value);
            
            if (isScheduleExist)
            {
                return (new ScheduleModel
                {
                    Start = existingStart!.Value,
                    End = existingEnd!.Value
                }, true);
            }
        }
        return (null!, false);
    }
}