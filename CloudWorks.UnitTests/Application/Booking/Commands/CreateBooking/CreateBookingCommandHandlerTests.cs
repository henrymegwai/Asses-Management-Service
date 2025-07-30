using System.Linq.Expressions;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Booking.Commands.CreateBooking;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandlerTests
{
    private readonly ILogger<CreateBookingCommandHandler> _logger;
    private readonly IValidator<CreateBookingCommand> _validator;
    private readonly IRepository<Data.Entities.Booking> _bookingRepository;
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<BookingAssessPoint> _bookingAccessPointRepository;
    private readonly IRepository<BookingSiteProfile> _bookingSiteProfileRepository;
    private readonly IRepository<Data.Entities.AccessPoint> _accessPointRepository;
    private readonly IRepository<Profile> _profileRepository;
    private readonly CreateBookingCommandHandler _sut;

    public CreateBookingCommandHandlerTests()
    {
        _logger = Substitute.For<ILogger<CreateBookingCommandHandler>>();
        _validator = Substitute.For<IValidator<CreateBookingCommand>>();
        _bookingRepository = Substitute.For<IRepository<Data.Entities.Booking>>();
        _scheduleRepository = Substitute.For<IRepository<Schedule>>();
        _bookingAccessPointRepository = Substitute.For<IRepository<BookingAssessPoint>>();
        _bookingSiteProfileRepository = Substitute.For<IRepository<BookingSiteProfile>>();
        _accessPointRepository = Substitute.For<IRepository<Data.Entities.AccessPoint>>();
        _profileRepository = Substitute.For<IRepository<Profile>>();
        _sut = new CreateBookingCommandHandler(
            _logger, _validator, _bookingRepository, _scheduleRepository,
            _bookingAccessPointRepository, _bookingSiteProfileRepository,
            _accessPointRepository, _profileRepository);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateBookingCommand(
            Guid.NewGuid(), "", new List<string>(), new List<Guid>(), new List<ScheduleModel>());
        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required")
        };
        var validationResult = new ValidationResult(validationFailures);
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Validation failed.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenScheduleExists_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var startTime1 = DateTime.UtcNow.AddHours(1);
        var endTime1 = DateTime.UtcNow.AddHours(2);
        var startTime2 = DateTime.UtcNow.AddHours(3);
        var endTime2 = DateTime.UtcNow.AddHours(4);
        var command = new CreateBookingCommand(
            siteId, "Test Booking", [], [Guid.NewGuid()],
            [new ScheduleModel { Start = startTime1, End = endTime1 }]);
        var validationResult = new ValidationResult();
        
        var e1 = new CalendarEvent
        {
            Start = new CalDateTime(startTime1),
            End = new CalDateTime(endTime1)
        };
        var calendar = new Ical.Net.Calendar();
        calendar.Events.Add(e1);
        var serializer = new CalendarSerializer();
        var value1 = serializer.SerializeToString(calendar);
        
        var e2 = new CalendarEvent
        {
            Start = new CalDateTime(startTime2),
            End = new CalDateTime(endTime2)
        };
        var calendar2 = new Ical.Net.Calendar();
        calendar2.Events.Add(e2);
        var serializer2 = new CalendarSerializer();
        var value2 = serializer2.SerializeToString(calendar2);
        
        var existingSchedules = new List<Schedule>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                SiteId = siteId, 
                Value = value1!, 
                BookingId = bookingId 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                SiteId = siteId, 
                Value = value2!, 
                BookingId = bookingId 
            }
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _scheduleRepository.GetAllAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), 
                Arg.Any<Expression<Func<Schedule, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSchedules));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Schedule already exists for booking");
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateBookingSuccessfully()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var accessPointId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = DateTime.UtcNow.AddHours(2);
        var command = new CreateBookingCommand(
            siteId, "Test Booking", new List<string>(), new List<Guid> { accessPointId },
            new List<ScheduleModel> { new() { Start = startTime, End = endTime } });
        var validationResult = new ValidationResult();
        var newBooking = new Data.Entities.Booking
        {
            Id = Guid.NewGuid(),
            SiteId = siteId,
            Name = command.Name
        };
        var accessPoints = new List<Data.Entities.AccessPoint>
        {
            new() { Id = accessPointId, SiteId = siteId, Name = "Test Access Point" }
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _scheduleRepository.GetAllAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<Schedule>()));
        
        _bookingRepository.AddAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(newBooking));
        
        _scheduleRepository.AddRangeAsync(Arg.Any<List<Schedule>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());
        
        _accessPointRepository.GetAllAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<Expression<Func<Data.Entities.AccessPoint, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(accessPoints));
        
        _bookingAccessPointRepository.AddRangeAsync(Arg.Any<List<BookingAssessPoint>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.Name);
        result.Message.Should().Be("Booking created successfully.");
        
        await _bookingRepository.Received(1).AddAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>());
        await _scheduleRepository.Received(1).AddRangeAsync(Arg.Any<List<Schedule>>(), Arg.Any<CancellationToken>());
        await _bookingAccessPointRepository.Received(1).AddRangeAsync(Arg.Any<List<BookingAssessPoint>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBookingCreationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new CreateBookingCommand(
            siteId, "Test Booking", new List<string>(), new List<Guid>(),
            new List<ScheduleModel> { new() { Start = DateTime.UtcNow.AddHours(1), End = DateTime.UtcNow.AddHours(2) } });
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _scheduleRepository.GetAllAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<Schedule>()));
        
        _bookingRepository.AddAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Booking>("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to create booking.");
    }

    [Fact]
    public async Task Handle_WhenUsersEmailsProvided_ShouldCreateBookingSiteProfiles()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var userEmail = "admin@cloudworks.com";
        var command = new CreateBookingCommand(
            siteId, "Test Booking", [userEmail], new List<Guid>(),
            [new() { Start = DateTime.UtcNow.AddHours(1), End = DateTime.UtcNow.AddHours(2) }]);
        var validationResult = new ValidationResult();
        var newBooking = new Data.Entities.Booking
        {
            Id = Guid.NewGuid(),
            SiteId = siteId,
            Name = command.Name
        };
        var profiles = new List<Profile>
        {
            new() { Id = Guid.NewGuid(), Email = userEmail }
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _scheduleRepository.GetAllAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<Schedule>()));
        
        _bookingRepository.AddAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(newBooking));
        
        _scheduleRepository.AddRangeAsync(Arg.Any<List<Schedule>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());
        
        _accessPointRepository.GetAllAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<Expression<Func<Data.Entities.AccessPoint, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<Data.Entities.AccessPoint>()));
        
        _bookingAccessPointRepository.AddRangeAsync(Arg.Any<List<BookingAssessPoint>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());
        
        _profileRepository.GetAllAsync(Arg.Any<Expression<Func<Profile, bool>>>(), Arg.Any<Expression<Func<Profile, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(profiles));
        
        _bookingSiteProfileRepository.AddRangeAsync(Arg.Any<List<BookingSiteProfile>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Booking created successfully.");
        
        await _profileRepository.Received(1).GetAllAsync(Arg.Any<Expression<Func<Profile, bool>>>(), Arg.Any<Expression<Func<Profile, object>>[]>(), Arg.Any<CancellationToken>());
        await _bookingSiteProfileRepository.Received(1).AddRangeAsync(Arg.Any<List<BookingSiteProfile>>(), Arg.Any<CancellationToken>());
    }
} 