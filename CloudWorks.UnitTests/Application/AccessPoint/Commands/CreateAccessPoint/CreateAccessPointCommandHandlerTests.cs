using System.Linq.Expressions;
using CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.AccessPoint.Commands.CreateAccessPoint;

public class CreateAccessPointCommandHandlerTests
{
    private readonly IRepository<Data.Entities.AccessPoint> _repository;
    private readonly IValidator<CreateAccessPointCommand> _validator;
    private readonly ILogger<CreateAccessPointCommandHandler> _logger;
    private readonly CreateAccessPointCommandHandler _sut;

    public CreateAccessPointCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.AccessPoint>>();
        _validator = Substitute.For<IValidator<CreateAccessPointCommand>>();
        _logger = Substitute.For<ILogger<CreateAccessPointCommandHandler>>();
        _sut = new CreateAccessPointCommandHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateAccessPointCommand("Test Access Point", Guid.NewGuid());
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
    public async Task Handle_WhenAccessPointWithSameNameExists_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateAccessPointCommand("Existing Access Point", Guid.NewGuid());
        var existingAccessPoint = new Data.Entities.AccessPoint
        {
            Id = Guid.NewGuid(),
            Name = "Existing Access Point",
            SiteId = command.SiteId
        };
        
        var validationResult = new ValidationResult();
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingAccessPoint));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("An access point with the same name already exists.");
    }

    [Fact]
    public async Task Handle_WhenAccessPointDoesNotExist_ShouldCreateNewAccessPoint()
    {
        // Arrange
        var command = new CreateAccessPointCommand("New Access Point", Guid.NewGuid());
        var validationResult = new ValidationResult();
        var newAccessPoint = new Data.Entities.AccessPoint
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            SiteId = command.SiteId,
            CreatedAt = DateTime.Now,
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.AccessPoint>(null!));
        
        _repository.AddAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(newAccessPoint));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.Name);
        result.Data.SiteId.Should().Be(command.SiteId);
        result.Message.Should().Be("Access point created successfully.");
        
        await _repository.Received(1).AddAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryAddFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateAccessPointCommand("New Access Point", Guid.NewGuid());
        var validationResult = new ValidationResult();
        var error = Result.Fail("Database error");
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.AccessPoint>(null!));
        
        _repository.AddAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to create access point.");
    }

    [Fact]
    public async Task Handle_WhenRepositoryGetByThrowsException_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateAccessPointCommand("New Access Point", Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("GetBy operation failed."));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("An error occurred while checking for existing access points.");
    }
} 