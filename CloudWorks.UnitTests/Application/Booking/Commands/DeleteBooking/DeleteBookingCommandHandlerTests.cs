using System.Linq.Expressions;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Booking.Commands.DeleteBooking;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;

namespace CloudWorks.UnitTests.Application.Booking.Commands.DeleteBooking;

public class DeleteBookingCommandHandlerTests
{
    private readonly IRepository<Data.Entities.Booking> _repository;
    private readonly IValidator<DeleteBookingCommand> _validator;
    private readonly DeleteBookingCommandHandler _sut;

    public DeleteBookingCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.Booking>>();
        _validator = Substitute.For<IValidator<DeleteBookingCommand>>();
        _sut = new DeleteBookingCommandHandler(_repository, _validator);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteBookingCommand(Guid.Empty);
        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "Id is required")
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
        var command = new DeleteBookingCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Booking>(null!));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task Handle_WhenBookingExists_ShouldDeleteSuccessfully()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new DeleteBookingCommand(bookingId);
        var validationResult = new ValidationResult();
        var existingBooking = new Data.Entities.Booking
        {
            Id = bookingId,
            Name = "Test Booking",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingBooking));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Booking deleted successfully.");
        
        await _repository.Received(1).DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new DeleteBookingCommand(bookingId);
        var validationResult = new ValidationResult();
        var existingBooking = new Data.Entities.Booking
        {
            Id = bookingId,
            Name = "Test Booking",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingBooking));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database delete error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Booking deletion failed.");
    }

    [Fact]
    public async Task Handle_WhenGetByFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteBookingCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Booking>("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to retrieve booking.");
    }
} 