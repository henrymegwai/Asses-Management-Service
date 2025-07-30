using CloudWorks.Application.Features.Site.Commands.UpdateSite;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Site.Commands.UpdateSite;

public class UpdateSiteCommandHandlerTests
{
    private readonly IRepository<Data.Entities.Site> _repository;
    private readonly IValidator<UpdateSiteCommand> _validator;
    private readonly ILogger<UpdateSiteCommandHandler> _logger;
    private readonly UpdateSiteCommandHandler _sut;

    public UpdateSiteCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.Site>>();
        _validator = Substitute.For<IValidator<UpdateSiteCommand>>();
        _logger = Substitute.For<ILogger<UpdateSiteCommandHandler>>();
        _sut = new UpdateSiteCommandHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdateSiteCommand(Guid.NewGuid(), "");
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
    public async Task Handle_WhenSiteNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new UpdateSiteCommand(siteId, "Updated Site");
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Site>("Site not found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be($"Site with ID {command.Id} not found.");
    }

    [Fact]
    public async Task Handle_WhenNoChangesDetected_ShouldReturnSuccessResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var siteName = "Test Site";
        var command = new UpdateSiteCommand(siteId, siteName);
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = siteName,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(siteName);
        result.Message.Should().Be("No changes detected.");
    }

    [Fact]
    public async Task Handle_WhenSiteExists_ShouldUpdateSuccessfully()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new UpdateSiteCommand(siteId, "Updated Site");
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Original Site",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        var updatedSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = command.Name,
            CreatedAt = existingSite.CreatedAt,
            CreatedBy = existingSite.CreatedBy,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));
        
        _repository.UpdateAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(updatedSite));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(command.Id);
        result.Data.Name.Should().Be(command.Name);
        result.Message.Should().Be("Site updated successfully.");
        
        await _repository.Received(1).UpdateAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new UpdateSiteCommand(siteId, "Updated Site");
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Original Site",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));
        
        _repository.UpdateAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database update error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to update site.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }
} 