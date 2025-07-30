using CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.AccessPoint.Commands.UpdateAccessPoint;

public class UpdateAccessPointCommandHandlerTests
{
    private readonly IRepository<Data.Entities.AccessPoint> _repository;
    private readonly IValidator<UpdateAccessPointCommand> _validator;
    private readonly ILogger<UpdateAccessPointCommandHandler> _logger;
    private readonly UpdateAccessPointCommandHandler _sut;

    public UpdateAccessPointCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.AccessPoint>>();
        _validator = Substitute.For<IValidator<UpdateAccessPointCommand>>();
        _logger = Substitute.For<ILogger<UpdateAccessPointCommandHandler>>();
        _sut = new UpdateAccessPointCommandHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdateAccessPointCommand(Guid.NewGuid(), "", Guid.NewGuid());
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
    public async Task Handle_WhenAccessPointNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdateAccessPointCommand(Guid.NewGuid(), "Updated Name", Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.AccessPoint>("Access point not found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Access point not found.");
    }

    [Fact]
    public async Task Handle_WhenAccessPointExists_ShouldUpdateSuccessfully()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var command = new UpdateAccessPointCommand(accessPointId, "Updated Name", siteId);
        var validationResult = new ValidationResult();
        
        var existingAccessPoint = new Data.Entities.AccessPoint
        {
            Id = accessPointId,
            Name = "Original Name",
            SiteId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        
        var updatedAccessPoint = new Data.Entities.AccessPoint
        {
            Id = accessPointId,
            Name = command.Name,
            SiteId = command.SiteId,
            CreatedAt = existingAccessPoint.CreatedAt,
            CreatedBy = existingAccessPoint.CreatedBy,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingAccessPoint));
        
        _repository.UpdateAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(updatedAccessPoint));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(command.Id);
        result.Data.Name.Should().Be(command.Name);
        result.Data.SiteId.Should().Be(command.SiteId);
        result.Message.Should().Be("Access point updated successfully.");
        
        await _repository.Received(1).UpdateAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var command = new UpdateAccessPointCommand(accessPointId, "Updated Name", Guid.NewGuid());
        var validationResult = new ValidationResult();
        var existingAccessPoint = new Data.Entities.AccessPoint
        {
            Id = accessPointId,
            Name = "Original Name",
            SiteId = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingAccessPoint));
        
        _repository.UpdateAsync(Arg.Any<Data.Entities.AccessPoint>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database update error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to update access point.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenAccessPointIsNull_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdateAccessPointCommand(Guid.NewGuid(), "Updated Name", Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.AccessPoint>(null!));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Access point not found.");
    }
} 