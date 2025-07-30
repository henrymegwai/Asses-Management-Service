using CloudWorks.Api.Endpoints.Requests;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Features.Booking.Commands.CreateBooking;
using CloudWorks.Application.Features.Booking.Commands.DeleteBooking;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Features.Booking.Commands.UpdateBooking;
using CloudWorks.Application.Features.Booking.Queries.GetBooking;
using CloudWorks.Api;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CloudWorks.IntegrationTests.Api.Controllers;

[Collection("Controller Tests")]
public class BookingsControllerTests : TestBaseFactory
{
    private readonly IMediator _mediator;

    public BookingsControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _mediator = Substitute.For<IMediator>();
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task GetBookings_WhenValidParameters_ShouldReturnBookings()
    {
        // Arrange
        var expectedResponse = new Response<Page<BookingDto>>(
            true, 
            new Page<BookingDto>(new List<BookingDto>(), 1, 20, 0), 
            "Bookings retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetBookingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/bookings?name=Test&sortBy=Name&desc=false&page=1&pageSize=20");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task GetBookings_WhenInvalidParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var expectedResponse = new Response<Page<BookingDto>>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<GetBookingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/bookings?page=-1");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task GetBookings_WhenNoParameters_ShouldReturnBookings()
    {
        // Arrange
        var expectedResponse = new Response<Page<BookingDto>>(
            true, 
            new Page<BookingDto>(new List<BookingDto>(), 1, 20, 0), 
            "Bookings retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetBookingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/bookings");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task CreateBooking_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new AddBookingRequest
        {
            Name = "Test Booking",
            UsersEmails = new List<string> { "test@example.com" },
            AccessPoints = new List<Guid> { Guid.NewGuid() },
            Schedules = new List<ScheduleRequest>
            {
                new() { Start = DateTime.UtcNow.AddHours(1), End = DateTime.UtcNow.AddHours(2) }
            }
        };
        var expectedResponse = new Response<BookingDto>(
            true, 
            new BookingDto(request.Name, new List<string>(), new List<Guid>(), new List<ScheduleModel>()), 
            "Booking created successfully.");
        
        _mediator.Send(Arg.Any<CreateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/bookings/{siteId}/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task CreateBooking_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new AddBookingRequest
        {
            Name = "",
            UsersEmails = new List<string>(),
            AccessPoints = new List<Guid>(),
            Schedules = new List<ScheduleRequest>()
        };
        var expectedResponse = new Response<BookingDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<CreateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/bookings/{siteId}/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task CreateBooking_WhenEmptySchedules_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new AddBookingRequest
        {
            Name = "Test Booking",
            UsersEmails = new List<string> { "test@example.com" },
            AccessPoints = new List<Guid> { Guid.NewGuid() },
            Schedules = new List<ScheduleRequest>()
        };
        var expectedResponse = new Response<BookingDto>(
            false, 
            null!, 
            "At least one schedule is required.");
        
        _mediator.Send(Arg.Any<CreateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/bookings/{siteId}/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateBooking_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var request = new UpdateBookingRequest
        {
            Name = "Updated Booking",
            SiteId = Guid.NewGuid()
        };
        var expectedResponse = new Response<BookingDto>(
            true, 
            new BookingDto(request.Name, new List<string>(), new List<Guid>(), new List<ScheduleModel>()), 
            "Booking updated successfully.");
        
        _mediator.Send(Arg.Any<UpdateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/bookings/{bookingId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateBooking_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var request = new UpdateBookingRequest
        {
            Name = "",
            SiteId = Guid.NewGuid()
        };
        var expectedResponse = new Response<BookingDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<UpdateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/bookings/{bookingId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateBooking_WhenBookingNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var request = new UpdateBookingRequest
        {
            Name = "Updated Booking",
            SiteId = Guid.NewGuid()
        };
        var expectedResponse = new Response<BookingDto>(
            false, 
            null!, 
            "Booking not found.");
        
        _mediator.Send(Arg.Any<UpdateBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/bookings/{bookingId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task DeleteBooking_WhenValidId_ShouldReturnSuccess()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            true, 
            "Booking deleted successfully.", 
            "Booking deleted successfully.");
        
        _mediator.Send(Arg.Any<DeleteBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/bookings/{bookingId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task DeleteBooking_WhenInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var bookingId = Guid.Empty;
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<DeleteBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/bookings/{bookingId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task DeleteBooking_WhenBookingNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Booking not found.");
        
        _mediator.Send(Arg.Any<DeleteBookingCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/bookings/{bookingId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
} 