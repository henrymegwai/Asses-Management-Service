using System.Linq.Expressions;
using CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.AccessPoint.Commands.FreeTimeSlot;

public class GetFreeTimeSlotsQueryHandlerTests
{
    private readonly IRepository<Data.Entities.Booking> _bookingRepository;
    private readonly IValidator<GetFreeTimeSlotsQuery> _validator;
    private readonly ILogger<GetFreeTimeSlotsQueryHandler> _logger;
    private readonly GetFreeTimeSlotsQueryHandler _sut;

    public GetFreeTimeSlotsQueryHandlerTests()
    {
        _bookingRepository = Substitute.For<IRepository<Data.Entities.Booking>>();
        _validator = Substitute.For<IValidator<GetFreeTimeSlotsQuery>>();
        _logger = Substitute.For<ILogger<GetFreeTimeSlotsQueryHandler>>();
        _sut = new GetFreeTimeSlotsQueryHandler(_bookingRepository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var query = new GetFreeTimeSlotsQuery(
            Guid.Empty, 
            new CalDateTime(DateTime.UtcNow), 
            new CalDateTime(DateTime.UtcNow.AddHours(1)), 
            30);
        var validationFailures = new List<ValidationFailure>
        {
            new("AccessPointId", "AccessPointId is required")
        };
        var validationResult = new ValidationResult(validationFailures);
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Validation failed.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNoBookingsExist_ShouldReturnAllTimeAsFree()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var from = new CalDateTime(DateTime.UtcNow);
        var to = new CalDateTime(DateTime.UtcNow.AddHours(2));
        var query = new GetFreeTimeSlotsQuery(accessPointId, from, to, 30);
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetAllAsync( Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(),
            Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>(),
            Arg.Any<CancellationToken>()).Returns(new Result<List<Data.Entities.Booking>>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data[0].Start.Should().Be(from);
        result.Data[0].End.Should().Be(to);
        result.Message.Should().Be("No bookings found, returning the entire range as a free slot.");
    }

    [Fact]
    public async Task Handle_WhenBookingsExist_ShouldReturnFreeSlots()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var from = new CalDateTime(DateTime.UtcNow);
        var to = new CalDateTime(DateTime.UtcNow.AddHours(4));
        var query = new GetFreeTimeSlotsQuery(accessPointId, from, to, 30);
        var validationResult = new ValidationResult();
        
        // Create a booking that occupies the middle hour
        var bookingStart = new CalDateTime(DateTime.UtcNow.AddHours(1));
        var bookingEnd = new CalDateTime(DateTime.UtcNow.AddHours(2));
        
        var e = new CalendarEvent
        {
            Start = new CalDateTime(bookingStart),
            End = new CalDateTime(bookingEnd)
        };
        var calendar = new Ical.Net.Calendar();
        calendar.Events.Add(e);
        var serializer = new CalendarSerializer();
        var value = serializer.SerializeToString(calendar);
        
        var bookings = new List<Data.Entities.Booking>
        {
            new()
            {
                Id = bookingId,
                Name = "Hall Booking",
                SiteId = siteId,
                AccessPoints = new List<Data.Entities.AccessPoint>
                {
                    new() { Id = accessPointId, Name = "Cross Access Point", SiteId = siteId }
                },
                Schedules = new List<Schedule>
                {
                    new() { Id = Guid.NewGuid(), Value = value!, BookingId  = bookingId, SiteId = siteId}
                }
            },
            new()
            {
                Id = bookingId,
                Name = "Stage Booking Tests",
                SiteId = siteId,
                AccessPoints = new List<Data.Entities.AccessPoint>
                {
                    new() { Id = accessPointId, Name = "Even Access Point", SiteId = siteId }
                },
                Schedules = new List<Schedule>
                {
                    new() { Id = Guid.NewGuid(), Value = value!, BookingId  = bookingId, SiteId = siteId}
                }
            },
            new()
            {
                Id = bookingId,
                Name = "Glow Booking Tests",
                SiteId = siteId,
                AccessPoints = new List<Data.Entities.AccessPoint>
                {
                    new() { Id = accessPointId, Name = "Gate Access Point", SiteId = siteId }
                },
                Schedules = new List<Schedule>
                {
                    new() { Id = Guid.NewGuid(), Value = value!, BookingId  = bookingId, SiteId = siteId}
                }
            }
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetAllAsync( Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(),
            Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>(),
            Arg.Any<CancellationToken>()).Returns(bookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Free time slots retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenDurationTooLong_ShouldFilterOutShortSlots()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var from = new CalDateTime(DateTime.UtcNow);
        var to = new CalDateTime(DateTime.UtcNow.AddHours(1));
        var query = new GetFreeTimeSlotsQuery(accessPointId, from, to, 30); // 2 hours minimum
        var validationResult = new ValidationResult();
        
        // Create a booking that leaves only 30 minutes free
        var bookingStart = new CalDateTime(DateTime.UtcNow.AddMinutes(30));
        var bookingEnd = new CalDateTime(DateTime.UtcNow.AddHours(1));
        
        var e = new CalendarEvent
        {
            Start = new CalDateTime(bookingStart),
            End = new CalDateTime(bookingEnd)
        };
        var calendar = new Ical.Net.Calendar();
        calendar.Events.Add(e);
        var serializer = new CalendarSerializer();
        var value = serializer.SerializeToString(calendar);
        
        var bookings = new List<Data.Entities.Booking>
        {
            new()
            {
                Id = bookingId,
                Name = "Booking Tests",
                SiteId = siteId,
                AccessPoints = new List<Data.Entities.AccessPoint>
                {
                    new() { Id = accessPointId, Name = "Even Access Point", SiteId = siteId }
                },
                Schedules = new List<Schedule>
                {
                    new() { Id = Guid.NewGuid(), Value = value!, BookingId  = bookingId, SiteId = siteId}
                }
            },
            new()
            {
                Id = bookingId,
                Name = "Another Booking Tests",
                SiteId = siteId,
                AccessPoints = new List<Data.Entities.AccessPoint>
                {
                    new() { Id = accessPointId, Name = "Gate Access Point", SiteId = siteId }
                },
                Schedules = new List<Schedule>
                {
                    new() { Id = Guid.NewGuid(), Value = value!, BookingId  = bookingId, SiteId = siteId}
                }
            }
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetAllAsync( Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(),
            Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>(),
            Arg.Any<CancellationToken>()).Returns(bookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Should not include the 30-minute slot since minimum duration is 2 hours
        result.Data.Count().Should().Be(1);
        result.Message.Should().Be("Free time slots retrieved successfully.");
    }
} 