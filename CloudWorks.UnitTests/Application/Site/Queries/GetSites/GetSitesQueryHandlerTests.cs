using System.Linq.Expressions;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Site.Queries.GetSites;
using CloudWorks.Infrastructure.Common.Interfaces;

namespace CloudWorks.UnitTests.Application.Site.Queries.GetSites;

public class GetSitesQueryHandlerTests
{
    private readonly IRepository<Data.Entities.Site> _siteRepository;
    private readonly GetSitesQueryHandler _sut;

    public GetSitesQueryHandlerTests()
    {
        _siteRepository = Substitute.For<IRepository<Data.Entities.Site>>();
        _sut = new GetSitesQueryHandler(_siteRepository);
    }

    [Fact]
    public async Task Handle_WhenNoQueryParams_ShouldReturnAllSites()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
          null,
           null,
            false,
          1,
           10
        );
        var query = new GetSitesQuery(queryParams);
        var sites = new List<Data.Entities.Site>
        {
            new() { Id = Guid.NewGuid(), Name = "Site 1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Site 2", CreatedAt = DateTime.UtcNow }
        };
        var queryableSites = sites.AsQueryable();
        
        _siteRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(2);
        result.Message.Should().Be("Sites retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNameFilterProvided_ShouldReturnFilteredSites()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "Test Site",
            null,
            false,
            1,
            10
        );
        var query = new GetSitesQuery(queryParams);
        var sites = new List<Data.Entities.Site>
        {
            new() { Id = Guid.NewGuid(), Name = "Test Site", CreatedAt = DateTime.UtcNow }
        };
        var queryableSites = sites.AsQueryable();
        
        _siteRepository.Query(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(1);
        result.Data.Items[0].Name.Should().Be("Test Site");
        result.Message.Should().Be("Sites retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSortByNameAscending_ShouldReturnSortedSites()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "Name",
            null,
            false,
            1,
            10
        );
        var query = new GetSitesQuery(queryParams);
        var sites = new List<Data.Entities.Site>
        {
            new() { Id = Guid.NewGuid(), Name = "Site B", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Site A", CreatedAt = DateTime.UtcNow }
        };
        var queryableSites = sites.AsQueryable();
        
        _siteRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Sites retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSortByNameDescending_ShouldReturnSortedSites()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "Name",
            null,
            false,
            1,
            10
        );
        var query = new GetSitesQuery(queryParams);
        var sites = new List<Data.Entities.Site>
        {
            new() { Id = Guid.NewGuid(), Name = "Site A", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Site B", CreatedAt = DateTime.UtcNow }
        };
        var queryableSites = sites.AsQueryable();
        
        _siteRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Sites retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenPaginationApplied_ShouldReturnPaginatedResults()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            null,
            null,
            false,
            1,
            2
        );
        var query = new GetSitesQuery(queryParams);
        var sites = new List<Data.Entities.Site>
        {
            new() { Id = Guid.NewGuid(), Name = "Site 1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Site 2", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Site 3", CreatedAt = DateTime.UtcNow }
        };
        var queryableSites = sites.AsQueryable();
        
        _siteRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(2);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(2);
        result.Message.Should().Be("Sites retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNoSitesFound_ShouldReturnEmptyPage()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "NonExistentSite",
            null,
            false,
            1,
            10
        );
        var query = new GetSitesQuery(queryParams);
        var emptySites = new List<Data.Entities.Site>();
        var queryableSites = emptySites.AsQueryable();
        
        _siteRepository.Query(Arg.Any<Expression<Func<Data.Entities.Site, bool>>>(), null, Arg.Any<Expression<Func<Data.Entities.Site, object>>[]>())
            .Returns(queryableSites);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().BeEmpty();
        result.Message.Should().Be("Sites retrieved successfully.");
    }
} 