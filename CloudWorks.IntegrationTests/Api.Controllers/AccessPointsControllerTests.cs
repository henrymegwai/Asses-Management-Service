using CloudWorks.Api.Endpoints.Requests;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;
using CloudWorks.Application.Features.AccessPoint.Commands.OpenAccessPoint.Commands;
using CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;
using CloudWorks.Api;
using Ical.Net.DataTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CloudWorks.IntegrationTests.Api.Controllers;

[Collection("Controller Tests")]
public class AccessPointsControllerTests(WebApplicationFactory<Program> factory) : TestBaseFactory(factory)
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task Get_WhenValidSiteId_ShouldReturnAccessPoints()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<List<AccessPointDto>>(
            true,
            [
                new(Guid.NewGuid(), "Test Access Point", siteId,
                    new SiteDto(Guid.NewGuid(), "Test Site", new List<ProfileDto>()))
            ], 
            "Access points retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetAccessPointsBySiteIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/accesspoints/{siteId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task Get_WhenInvalidSiteId_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.Empty;
        var expectedResponse = new Response<List<AccessPointDto>>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<GetAccessPointsBySiteIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/accesspoints/{siteId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task GetAccessPointHistories_WhenValidParameters_ShouldReturnHistories()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddDays(-1);
        var end = DateTime.UtcNow;
        var expectedResponse = new Response<Page<AccessPointHistoryDto>>(
            true, 
            new Page<AccessPointHistoryDto>(new List<AccessPointHistoryDto>(), 1, 20, 0), 
            "Access point histories retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetAccessPointHistoriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/accesspoints/histories/{siteId}?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task GetFreeTimeSlots_WhenValidParameters_ShouldReturnFreeSlots()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var startTime = new CalDateTime(DateTime.UtcNow);
        var endTime = new CalDateTime(DateTime.UtcNow.AddHours(2));
        var expectedResponse = new Response<List<TimeSlotDto>>(
            true, 
            new List<TimeSlotDto> 
            { 
                new() { Start = startTime, End = endTime } 
            }, 
            "Free time slots retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetFreeTimeSlotsQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/accesspoints/{accessPointId}/free-time-slots?startTime={startTime}&endTime={endTime}&durationMinutes=30");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task CreateAccessPoint_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new CreateAccessPointRequest { Name = "Test Access Point" , SiteId = siteId };
        var expectedResponse = new Response<AccessPointDto>(
            true, 
            new AccessPointDto(Guid.NewGuid(), request.Name, siteId, new SiteDto(siteId, "Test Site", [ 
                new ProfileDto(Guid.NewGuid(), "email@email.com", Guid.NewGuid())])), 
            "Access point created successfully.");
        
          _mediator.Send(new CreateAccessPointCommand(request.Name, request.SiteId), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/accesspoints/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task CreateAccessPoint_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAccessPointRequest { Name = "" };
        var expectedResponse = new Response<AccessPointDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<CreateAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/accesspoints/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateAccessPoint_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var request = new UpdateAccessPointRequest 
        { 
            Name = "Updated Access Point", 
            SiteId = Guid.NewGuid() 
        };
        var expectedResponse = new Response<AccessPointDto>(
            true, 
            new AccessPointDto(accessPointId, request.Name, request.SiteId, new SiteDto(Guid.NewGuid(), "Test Site", new List<ProfileDto>())), 
            "Access point updated successfully.");
        
        _mediator.Send(Arg.Any<UpdateAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/accesspoints/{accessPointId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateAccessPoint_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var request = new UpdateAccessPointRequest 
        { 
            Name = "", 
            SiteId = Guid.NewGuid() 
        };
        var expectedResponse = new Response<AccessPointDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<UpdateAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/accesspoints/{accessPointId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task OpenAccessPoint_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var request = new OpenAccessPointRequest 
        { 
            ProfileId = Guid.NewGuid(), 
            SiteId = Guid.NewGuid() 
        };
        var expectedResponse = new Response<OpenAccessDto>(
            true, 
            new OpenAccessDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Test Access Point", 
                SiteId = Guid.NewGuid(), 
                Site = new SiteDto(Guid.NewGuid(), "Test Site", new List<ProfileDto>()), 
                AccessPointStatus = "Successful", 
                OpenedAt = DateTime.UtcNow, 
                ClosedAt = null 
            }, 
            "Access point opened successfully.");
        
        _mediator.Send(Arg.Any<OpenAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/accesspoints/{accessPointId}/open", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task OpenAccessPoint_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var request = new OpenAccessPointRequest 
        { 
            ProfileId = Guid.Empty, 
            SiteId = Guid.NewGuid() 
        };
        var expectedResponse = new Response<OpenAccessDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<OpenAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync($"/accesspoints/{accessPointId}/open", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task DeleteAccessPoint_WhenValidId_ShouldReturnSuccess()
    {
        // Arrange
        var accessPointId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            true, 
            "Access Point deleted successfully.", 
            "Access Point deleted successfully.");
        
        _mediator.Send(Arg.Any<DeleteAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/accesspoints/{accessPointId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task DeleteAccessPoint_WhenInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var accessPointId = Guid.Empty;
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<DeleteAccessPointCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/accesspoints/{accessPointId}");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
} 