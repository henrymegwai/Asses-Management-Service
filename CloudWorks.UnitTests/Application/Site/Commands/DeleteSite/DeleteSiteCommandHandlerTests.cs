using System.Linq.Expressions;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Site.Commands.DeleteSite;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Site.Commands.DeleteSite;

public class DeleteSiteCommandHandlerTests
{
    private readonly IRepository<Data.Entities.Site> _repository;
    private readonly ILogger<DeleteSiteCommandHandler> _logger;
    private readonly IValidator<DeleteSiteCommand> _validator;
    private readonly DeleteSiteCommandHandler _handler;

    public DeleteSiteCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.Site>>();
        _logger = Substitute.For<ILogger<DeleteSiteCommandHandler>>();
        _validator = Substitute.For<IValidator<DeleteSiteCommand>>();
        _handler = new DeleteSiteCommandHandler(_repository, _logger, _validator);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteSiteCommand(Guid.Empty);
        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "Id is required")
        };
        var validationResult = new ValidationResult(validationFailures);
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

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
        var command = new DeleteSiteCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Site>(null!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Site not found.");
    }

    [Fact]
    public async Task Handle_WhenSiteExists_ShouldDeleteSuccessfully()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new DeleteSiteCommand(siteId);
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Test Site",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Site deleted successfully.");
        
        await _repository.Received(1).DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var command = new DeleteSiteCommand(siteId);
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Test Site",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));
        
        _repository.DeleteAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Database delete error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Site deletion failed.");
    }

    [Fact]
    public async Task Handle_WhenGetByIdFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new DeleteSiteCommand(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Site>("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to retrieve site.");
    }
} 