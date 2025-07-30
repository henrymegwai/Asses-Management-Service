using System.Linq.Expressions;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;

namespace CloudWorks.UnitTests.Application.AccessPoint.Commands.DeleteAccessPoint;

public class DeleteAccessPointCommandHandlerTests
{
    private readonly IRepository<Data.Entities.AccessPoint> _repository;
    private readonly IValidator<DeleteAccessPointCommand> _validator;
    private readonly DeleteAccessPointCommandHandler _sut;

    public DeleteAccessPointCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.AccessPoint>>();
        _validator = Substitute.For<IValidator<DeleteAccessPointCommand>>();
        _sut = new DeleteAccessPointCommandHandler(_repository, _validator);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteAccessPointCommand(Guid.Empty);
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
    public async Task Handle_WhenAccessPointNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteAccessPointCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.AccessPoint>(null!));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Access Point not found.");
    }

    [Fact]
    public async Task Handle_WhenAccessPointExists_ShouldDeleteSuccessfully()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var command = new DeleteAccessPointCommand(accessPointId);
        var validationResult = new ValidationResult();
        var existingAccessPoint = new Data.Entities.AccessPoint
        {
            Id = accessPointId,
            Name = "Test Access Point",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingAccessPoint));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Access Point deleted successfully.");
        
        await _repository.Received(1).DeleteAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var command = new DeleteAccessPointCommand(accessPointId);
        var validationResult = new ValidationResult();
        var existingAccessPoint = new Data.Entities.AccessPoint
        {
            Id = accessPointId,
            Name = "Test Access Point",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingAccessPoint));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database delete error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Access Point deletion failed.");
    }

    [Fact]
    public async Task Handle_WhenGetByFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteAccessPointCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.AccessPoint>("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("An error occurred while checking for the access point.");
    }
} 