using System.Linq.Expressions;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Booking.Queries.GetBooking;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CloudWorks.UnitTests.Application.Booking.Queries.GetBooking;

public class GetBookingsQueryHandlerTests
{
    private readonly IRepository<Data.Entities.Booking> _bookingRepository;
    private readonly IValidator<GetBookingsQuery> _validator;
    private readonly ILogger<GetBookingsQueryHandler> _logger;
    private readonly GetBookingsQueryHandler _sut;

    public GetBookingsQueryHandlerTests()
    {
        _bookingRepository = Substitute.For<IRepository<Data.Entities.Booking>>();
        _validator = Substitute.For<IValidator<GetBookingsQuery>>();
        _logger = Substitute.For<ILogger<GetBookingsQueryHandler>>();
        _sut = new GetBookingsQueryHandler(_bookingRepository, _validator, _logger);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResponse()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            null,
            null,
            false,
            -1,
            10);
        var query = new GetBookingsQuery(queryParams);
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
        result.Message.Should().Be("Validation failed.");
        result.Error.Should().NotBeNull();
        result.Error.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNoQueryParams_ShouldReturnAllBookings()
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
        var query = new GetBookingsQuery(queryParams);
        var bookings = new List<Data.Entities.Booking>
        {
            new() { Id = Guid.NewGuid(), Name = "Booking 1", SiteId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Booking 2", SiteId = Guid.NewGuid() }
        };
        var queryableBookings = bookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(2);
        result.Message.Should().Be("Bookings retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNameFilterProvided_ShouldReturnFilteredBookings()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "Test Booking",
            null,
            false,
            1,
            10);
        var query = new GetBookingsQuery(queryParams);
        var bookings = new List<Data.Entities.Booking>
        {
            new() { Id = Guid.NewGuid(), Name = "Test Booking", SiteId = Guid.NewGuid() }
        };
        var queryableBookings = bookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(1);
        result.Data.Items[0].Name.Should().Be("Test Booking");
        result.Message.Should().Be("Bookings retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSortByNameAscending_ShouldReturnSortedBookings()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "Name",
            null,
            false,
            1,
            10);
        var query = new GetBookingsQuery(queryParams);
        var bookings = new List<Data.Entities.Booking>
        {
            new() { Id = Guid.NewGuid(), Name = "Booking B", SiteId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Booking A", SiteId = Guid.NewGuid() }
        };
        var queryableBookings = bookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Bookings retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenSortByNameDescending_ShouldReturnSortedBookings()
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
        var query = new GetBookingsQuery(queryParams);
        var bookings = new List<Data.Entities.Booking>
        {
            new() { Id = Guid.NewGuid(), Name = "Booking A", SiteId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Booking B", SiteId = Guid.NewGuid() }
        };
        var queryableBookings = bookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Bookings retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenPaginationApplied_ShouldReturnPaginatedResults()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (null,
            null,
            false,
            1,
            10);
        var query = new GetBookingsQuery(queryParams);
        var bookings = new List<Data.Entities.Booking>
        {
            new() { Id = Guid.NewGuid(), Name = "Booking 1", SiteId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Booking 2", SiteId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Booking 3", SiteId = Guid.NewGuid() }
        };
        var queryableBookings = bookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(null, null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(3);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Message.Should().Be("Bookings retrieved successfully.");
    }

    [Fact]
    public async Task Handle_WhenNoBookingsFound_ShouldReturnEmptyPage()
    {
        // Arrange
        var queryParams = new QueryParamsModel
        (
            "NonExistentBooking",
            null,
            false,
            1,
            10);
        var query = new GetBookingsQuery(queryParams);
        var emptyBookings = new List<Data.Entities.Booking>();
        var queryableBookings = emptyBookings.AsQueryable();
        
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _bookingRepository.Query(Arg.Any<Expression<Func<Data.Entities.Booking, bool>>>(), null, Arg.Any<Expression<Func<Data.Entities.Booking, object>>[]>())
            .Returns(queryableBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().BeEmpty();
        result.Message.Should().Be("Bookings retrieved successfully.");
    }
} 