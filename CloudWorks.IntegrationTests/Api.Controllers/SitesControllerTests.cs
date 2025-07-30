using CloudWorks.Api.Endpoints.Requests;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Features.Site.Commands.CreateSite;
using CloudWorks.Application.Features.Site.Commands.DeleteSite;
using CloudWorks.Application.Features.Site.Commands.UpdateSite;
using CloudWorks.Application.Features.Site.Queries.GetSites;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Features.Site.Queries.GetUsersInSite;
using CloudWorks.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CloudWorks.IntegrationTests.Api.Controllers;

[Collection("Controller Tests")]
public class SitesControllerTests : TestBaseFactory
{
    private readonly IMediator _mediator;

    public SitesControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _mediator = Substitute.For<IMediator>();
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task Get_WhenValidParameters_ShouldReturnSites()
    {
        // Arrange
        var expectedResponse = new Response<Page<SiteDto>>(
            true, 
            new Page<SiteDto>(new List<SiteDto>(), 1, 20, 0), 
            "Sites retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetSitesQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/sites?name=Test&sortBy=Name&desc=false&page=1&pageSize=20");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task Get_WhenInvalidParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var expectedResponse = new Response<Page<SiteDto>>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<GetSitesQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/sites?page=-1");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task Get_WhenNoParameters_ShouldReturnSites()
    {
        // Arrange
        var expectedResponse = new Response<Page<SiteDto>>(
            true, 
            new Page<SiteDto>(new List<SiteDto>(), 1, 20, 0), 
            "Sites retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetSitesQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync("/sites");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task GetUsersInSite_WhenValidSiteId_ShouldReturnUsers()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<SiteDto>(
            true, 
            new SiteDto(Guid.NewGuid(), "Test Site", new List<ProfileDto>()), 
            "Users in site retrieved successfully.");
        
        _mediator.Send(Arg.Any<GetUsersInSiteQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/sites/{siteId}/users");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsersInSite_WhenInvalidSiteId_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.Empty;
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<GetUsersInSiteQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/sites/{siteId}/users");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUsersInSite_WhenSiteNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Site not found.");
        
        _mediator.Send(Arg.Any<GetUsersInSiteQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await GetAsync($"/sites/{siteId}/users");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task CreateSite_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateSiteRequest { Name = "Test Site" };
        var expectedResponse = new Response<SiteDto>(
            true, 
            new SiteDto(Guid.NewGuid(), request.Name, new List<ProfileDto>()), 
            "Site created successfully.");
        
        _mediator.Send(Arg.Any<CreateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync("/sites/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSite_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateSiteRequest { Name = "" };
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<CreateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync("/sites/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSite_WhenSiteNameAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateSiteRequest { Name = "Existing Site" };
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Site with this name already exists.");
        
        _mediator.Send(Arg.Any<CreateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PostAsync("/sites/create", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Authoriation is breaking the test")]
    public async Task UpdateSite_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new CreateSiteRequest { Name = "Updated Site" };
        var expectedResponse = new Response<SiteDto>(
            true, 
            new SiteDto(siteId, request.Name, new List<ProfileDto>()), 
            "Site updated successfully.");
        
        _mediator.Send(Arg.Any<UpdateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/sites/{siteId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSite_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new CreateSiteRequest { Name = "" };
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<UpdateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/sites/{siteId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSite_WhenSiteNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new CreateSiteRequest { Name = "Updated Site" };
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Site not found.");
        
        _mediator.Send(Arg.Any<UpdateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/sites/{siteId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSite_WhenSiteNameAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var request = new CreateSiteRequest { Name = "Existing Site" };
        var expectedResponse = new Response<SiteDto>(
            false, 
            null!, 
            "Site with this name already exists.");
        
        _mediator.Send(Arg.Any<UpdateSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var content = CreateJsonContent(request);
        var response = await PutAsync($"/sites/{siteId}/update", content);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Need to wire up a test container for this test.")]
    public async Task DeleteSite_WhenValidId_ShouldReturnSuccess()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            true, 
            "Site deleted successfully.", 
            "Site deleted successfully.");
        
        _mediator.Send(Arg.Any<DeleteSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/sites/{siteId}/delete");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSite_WhenInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.Empty;
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Validation failed.");
        
        _mediator.Send(Arg.Any<DeleteSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/sites/{siteId}/delete");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteSite_WhenSiteNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Site not found.");
        
        _mediator.Send(Arg.Any<DeleteSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/sites/{siteId}/delete");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteSite_WhenSiteHasActiveBookings_ShouldReturnBadRequest()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var expectedResponse = new Response<string>(
            false, 
            null!, 
            "Cannot delete site with active bookings.");
        
        _mediator.Send(Arg.Any<DeleteSiteCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var response = await DeleteAsync($"/sites/{siteId}/delete");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
} 