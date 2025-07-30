using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Features.Booking.Commands.UpdateBooking;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Booking.Commands.UpdateBooking;

public class UpdateBookingCommandHandlerTests
{
    private readonly IRepository<Data.Entities.Booking> _bookingRepository;
    private readonly IValidator<UpdateBookingCommand> _validator;
    private readonly ILogger<UpdateBookingCommandHandler> _logger;
    private readonly UpdateBookingCommandHandler _sut;

    public UpdateBookingCommandHandlerTests()
    {
        _bookingRepository = Substitute.For<IRepository<Data.Entities.Booking>>();
        _validator = Substitute.For<IValidator<UpdateBookingCommand>>();
        _logger = Substitute.For<ILogger<UpdateBookingCommandHandler>>();
        _sut = new UpdateBookingCommandHandler(_bookingRepository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdateBookingCommand(Guid.NewGuid(), "", Guid.NewGuid());
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
    public async Task Handle_WhenBookingNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new UpdateBookingCommand(bookingId, "Updated Booking", Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Booking>("Booking not found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be($"Booking with Id {command.Id} not found.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenBookingExists_ShouldUpdateSuccessfully()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var command = new UpdateBookingCommand(bookingId, "Updated Booking", siteId);
        var validationResult = new ValidationResult();
        var existingBooking = new Data.Entities.Booking
        {
            Id = bookingId,
            Name = "Original Booking",
            SiteId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        var updatedBooking = new Data.Entities.Booking
        {
            Id = bookingId,
            Name = command.Name,
            SiteId = command.SiteId,
            CreatedAt = existingBooking.CreatedAt,
            CreatedBy = existingBooking.CreatedBy,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingBooking));
        
        _bookingRepository.UpdateAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(updatedBooking));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.Name);
        result.Message.Should().Be("Booking updated successfully.");
        
        await _bookingRepository.Received(1).UpdateAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new UpdateBookingCommand(bookingId, "Updated Booking", Guid.NewGuid());
        var validationResult = new ValidationResult();
        var existingBooking = new Data.Entities.Booking
        {
            Id = bookingId,
            Name = "Original Booking",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingBooking));
        
        _bookingRepository.UpdateAsync(Arg.Any<Data.Entities.Booking>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database update error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to update booking.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenBookingIsNull_ShouldReturnFailureResponse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new UpdateBookingCommand(bookingId, "Updated Booking", Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _bookingRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Booking>(null!));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be($"Booking with Id {command.Id} not found.");
    }
} 