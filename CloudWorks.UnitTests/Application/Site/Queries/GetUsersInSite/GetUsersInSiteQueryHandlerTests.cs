using System.Linq.Expressions;
using CloudWorks.Application.Features.Site.Queries.GetUsersInSite;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Site.Queries.GetUsersInSite;

public class GetUsersInSiteQueryHandlerTests
{
    private readonly IRepository<Data.Entities.Site> _siteRepository;
    private readonly IValidator<GetUsersInSiteQuery> _validator;
    private readonly ILogger<GetUsersInSiteQueryHandler> _logger;
    private readonly GetUsersInSiteQueryHandler _sut;

    public GetUsersInSiteQueryHandlerTests()
    {
        _siteRepository = Substitute.For<IRepository<Data.Entities.Site>>();
        _validator = Substitute.For<IValidator<GetUsersInSiteQuery>>();
        _logger = Substitute.For<ILogger<GetUsersInSiteQueryHandler>>();
        _sut = new GetUsersInSiteQueryHandler(_siteRepository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var query = new GetUsersInSiteQuery(Guid.Empty);
        var validationFailures = new List<ValidationFailure>
        {
            new("SiteId", "SiteId is required")
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
    public async Task Handle_WhenSiteNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var query = new GetUsersInSiteQuery(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _siteRepository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok<Data.Entities.Site>(null!));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Site not found.");
    }

    [Fact]
    public async Task Handle_WhenSiteExistsWithUsers_ShouldReturnSiteWithUsers()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetUsersInSiteQuery(siteId);
        var validationResult = new ValidationResult();
        var site = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Test Site",
            Profiles = new List<SiteProfile>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SiteId = siteId,
                    ProfileId = Guid.NewGuid(),
                    Profile = new Profile
                    {
                        Id = Guid.NewGuid(),
                        Email = "user1@example.com",
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    SiteId = siteId,
                    ProfileId = Guid.NewGuid(),
                    Profile = new Profile
                    {
                        Id = Guid.NewGuid(),
                        Email = "user2@example.com"
                    }
                }
            }
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _siteRepository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(site));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(siteId);
        result.Data.Name.Should().Be("Test Site");
        result.Data.Users.Should().HaveCount(2);
        result.Data.Users[0].Email.Should().Be("user1@example.com");
        result.Data.Users[1].Email.Should().Be("user2@example.com");
        result.Message.Should().Be("Users retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSiteExistsWithNoUsers_ShouldReturnSuccessWithNoUsersMessage()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetUsersInSiteQuery(siteId);
        var validationResult = new ValidationResult();
        var site = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Test Site",
            Profiles = new List<SiteProfile>()
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _siteRepository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(site));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("No users found in the site.");
    }

    [Fact]
    public async Task Handle_WhenSiteExistsWithNullProfiles_ShouldReturnSuccessWithNoUsersMessage()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetUsersInSiteQuery(siteId);
        var validationResult = new ValidationResult();
        var site = new Data.Entities.Site
        {
            Id = siteId,
            Name = "Test Site",
            Profiles = new List<SiteProfile>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SiteId = siteId,
                    ProfileId = Guid.NewGuid(),
                    Profile = null!
                }
            }
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _siteRepository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(site));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("No users found in the site.");
    }

    [Fact]
    public async Task Handle_WhenGetByFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var query = new GetUsersInSiteQuery(Guid.NewGuid());
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _siteRepository.GetByAsync(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<Data.Entities.Site>("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to retrieve site.");
    }
} 