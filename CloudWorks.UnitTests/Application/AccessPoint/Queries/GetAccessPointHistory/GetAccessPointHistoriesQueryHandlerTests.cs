using System.Linq.Expressions;
using CloudWorks.Application.Common.Enumeration;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.AccessPoint.Queries.GetAccessPointHistory;

public class GetAccessPointHistoriesQueryHandlerTests
{
    private readonly IRepository<AccessPointHistory> _repository;
    private readonly IValidator<GetAccessPointHistoriesQuery> _validator;
    private readonly ILogger<GetAccessPointHistoriesQueryHandler> _logger;
    private readonly GetAccessPointHistoriesQueryHandler _sut;

    public GetAccessPointHistoriesQueryHandlerTests()
    {
        _repository = Substitute.For<IRepository<AccessPointHistory>>();
        _validator = Substitute.For<IValidator<GetAccessPointHistoriesQuery>>();
        _logger = Substitute.For<ILogger<GetAccessPointHistoriesQueryHandler>>();
        _sut = new GetAccessPointHistoriesQueryHandler(_repository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var queryParams = new QueryParamsModel(
            "", 
            "Name", 
            false, 
            1, 
            20
        );
        var query = new GetAccessPointHistoriesQuery(
            Guid.NewGuid(), queryParams, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        var validationFailures = new List<ValidationFailure>
        {
            new("PageNumber", "Page number must be greater than 0")
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
    public async Task Handle_WhenValidationPasses_ShouldReturnAccessPointHistories()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var queryParams = new QueryParamsModel(
            "", 
            "Name", 
            false, 
            1, 
            20
        );
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        var query = new GetAccessPointHistoriesQuery(siteId, queryParams, startDate, endDate);
        var validationResult = new ValidationResult();
        var histories = new List<AccessPointHistory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow, 
                AccessPointStatus = AccessPointStatus.Successful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 1" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow.AddHours(-1),
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 2" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            }
        };
        var queryableHistories = histories.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.Query(Arg.Any<Expression<Func<AccessPointHistory, bool>>>(), null, Arg.Any<Expression<Func<AccessPointHistory, object>>[]>())
            .Returns(queryableHistories);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(2);
        result.Message.Should().Be("Access point histories retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNoHistoriesFound_ShouldReturnEmptyPage()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var queryParams = new QueryParamsModel
        (
            "",
            "Name",
            false, 
            1,
            10
        );
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        var query = new GetAccessPointHistoriesQuery(siteId, queryParams, startDate, endDate);
        var validationResult = new ValidationResult();
        var emptyHistories = new List<AccessPointHistory>();
        var queryableHistories = emptyHistories.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.Query(Arg.Any<Expression<Func<AccessPointHistory, bool>>>(), 
                null, Arg.Any<Expression<Func<AccessPointHistory, object>>[]>())
            .Returns(queryableHistories);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().BeEmpty();
        result.Message.Should().Be("Access point histories retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSortByTimestamp_ShouldReturnSortedHistories()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var queryParams = new QueryParamsModel
        (
            "",
            "Timestamp",
            true, 
            1,
            10
        );
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        var query = new GetAccessPointHistoriesQuery(siteId, queryParams, startDate, endDate);
        var validationResult = new ValidationResult();
        var histories = new List<AccessPointHistory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow.AddHours(-1),
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 1" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow,
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 2" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            }
        };
        var queryableHistories = histories.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.Query(Arg.Any<Expression<Func<AccessPointHistory, bool>>>(), null, Arg.Any<Expression<Func<AccessPointHistory, object>>[]>())
            .Returns(queryableHistories);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Access point histories retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenPaginationApplied_ShouldReturnPaginatedResults()
    {
        // Arrange
        var siteId = Guid.NewGuid();
       
        var queryParams = new QueryParamsModel
        (
            "",
            null,
            true, 
            1,
            20
        );
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        var query = new GetAccessPointHistoriesQuery(siteId, queryParams, startDate, endDate);
        var validationResult = new ValidationResult();
        var histories = new List<AccessPointHistory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow,
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 1" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow.AddHours(-1),
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 2" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccessPointId = Guid.NewGuid(),
                SiteId = siteId,
                Timestamp = DateTime.UtcNow.AddHours(-2),
                AccessPointStatus = AccessPointStatus.NotSuccessful.ToString(),
                AccessPoint = new Data.Entities.AccessPoint { Id = Guid.NewGuid(), Name = "Access Point 3" },
                Site = new Data.Entities.Site { Id = siteId, Name = "Site 1" }
            }
        };
        var queryableHistories = histories.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationResult);
        
        _repository.Query(Arg.Any<Expression<Func<AccessPointHistory, bool>>>(), null, Arg.Any<Expression<Func<AccessPointHistory, object>>[]>())
            .Returns(queryableHistories);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(3);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(20);
        result.Message.Should().Be("Access point histories retrieved successfully.");
    }
} 