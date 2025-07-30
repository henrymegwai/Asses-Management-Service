using System.Linq.Expressions;
using CloudWorks.Application.Features.Site.Commands.CreateSite;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Site.Commands.CreateSite;

public class CreateSiteCommandHandlerTests
{
    private readonly IRepository<Data.Entities.Site> _repository;
    private readonly IValidator<CreateSiteCommand> _validator;
    private readonly ILogger<CreateSiteCommandHandler> _logger;
    private readonly CreateSiteCommandHandler _sut;

    public CreateSiteCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.Site>>();
        _validator = Substitute.For<IValidator<CreateSiteCommand>>();
        _logger = Substitute.For<ILogger<CreateSiteCommandHandler>>();
        _sut = new CreateSiteCommandHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateSiteCommand("");
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
    public async Task Handle_WhenSiteWithSameNameExists_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateSiteCommand("Existing Site");
        var validationResult = new ValidationResult();
        var existingSite = new Data.Entities.Site
        {
            Id = Guid.NewGuid(),
            Name = "Existing Site",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(existingSite));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be($"Site with name {command.Name} already exists.");
    }

    [Fact]
    public async Task Handle_WhenSiteDoesNotExist_ShouldCreateNewSite()
    {
        // Arrange
        var command = new CreateSiteCommand("New Site");
        var validationResult = new ValidationResult();
        var newSite = new Data.Entities.Site
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Site>(null!));
        
        _repository.AddAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(newSite));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.Name);
        result.Message.Should().Be("Site created successfully.");
        
        await _repository.Received(1).AddAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryAddFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateSiteCommand("New Site");
        var validationResult = new ValidationResult();
        var error = Result.Fail<Data.Entities.Site>("Database error");
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Site>(null!));
        
        _repository.AddAsync(Arg.Any<Data.Entities.Site>(), Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to create site.");
    }

    [Fact]
    public async Task Handle_WhenRepositoryGetByFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new CreateSiteCommand("New Site");
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Site>("Database connection error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to check if site exists.");
    }
} 