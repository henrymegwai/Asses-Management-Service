using System.Linq.Expressions;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.AccessPoint.Queries.GetAccessPointsForSite;

public class GetAccessPointsBySiteIdQueryHandlerTests
{
    private readonly IRepository<Data.Entities.AccessPoint> _repository;
    private readonly IValidator<GetAccessPointsBySiteIdQuery> _validator;
    private readonly ILogger<GetAccessPointsBySiteIdQueryHandler> _logger;
    private readonly GetAccessPointsBySiteIdQueryHandler _sut;

    public GetAccessPointsBySiteIdQueryHandlerTests()
    {
        _repository = Substitute.For<IRepository<Data.Entities.AccessPoint>>();
        _validator = Substitute.For<IValidator<GetAccessPointsBySiteIdQuery>>();
        _logger = Substitute.For<ILogger<GetAccessPointsBySiteIdQueryHandler>>();
        _sut = new GetAccessPointsBySiteIdQueryHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var query = new GetAccessPointsBySiteIdQuery(Guid.Empty);
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
        result.Message.Should().Be("Validation failed");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldReturnAccessPoints()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetAccessPointsBySiteIdQuery(siteId);
        var validationResult = new ValidationResult();
        var accessPoints = new List<Data.Entities.AccessPoint>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Access Point 1",
                SiteId = siteId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid()
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Access Point 2",
                SiteId = siteId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid()
            }
        };
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetAllAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<Expression<Func<Data.Entities.AccessPoint, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(accessPoints));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Access Point 1");
        result.Data[1].Name.Should().Be("Access Point 2");
        result.Message.Should().Be("Access points retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNoAccessPointsFound_ShouldReturnEmptyList()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetAccessPointsBySiteIdQuery(siteId);
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetAllAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<Expression<Func<Data.Entities.AccessPoint, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<Data.Entities.AccessPoint>()));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Message.Should().Be("Access points retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenRepositoryFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var query = new GetAccessPointsBySiteIdQuery(siteId);
        var validationResult = new ValidationResult();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.GetAllAsync(Arg.Any<Expression<Func<Data.Entities.AccessPoint, bool>>>(), Arg.Any<Expression<Func<Data.Entities.AccessPoint, object>>[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<List<Data.Entities.AccessPoint>>("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Failed to retrieve access points.");
    }
} 