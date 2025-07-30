 using CloudWorks.Api.Endpoints.Requests;
 using CloudWorks.Application.Common.Authentication;
 using CloudWorks.Application.Common.Models;
 using CloudWorks.Application.Dtos;
 using CloudWorks.Application.Features.Site.Commands.CreateSite;
 using CloudWorks.Application.Features.Site.Commands.DeleteSite;
 using CloudWorks.Application.Features.Site.Commands.UpdateSite;
 using CloudWorks.Application.Features.Site.Queries.GetSites;
 using CloudWorks.Application.Features.Site.Queries.GetUsersInSite;
 using CloudWorks.Data.Entities;
 using MediatR;
 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;

namespace CloudWorks.Api.Endpoints;

[ApiController]
[Route("sites")]
public class SitesController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<List<Site>>> Get(
        [FromQuery] string? name,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] bool desc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetSitesQuery(
                new QueryParamsModel(name, sortBy, desc, page, pageSize)) ,cancellationToken);
        return Ok(result);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id:guid}/users")]
    public async Task<ActionResult<Response<SiteDto>>> GetUsersInSite(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsersInSiteQuery(id), cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = ScopeConstants.ScopeManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateSite(
        [FromBody] CreateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateSiteCommand(request.Name), cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}/update")]
    public async Task<IActionResult> UpdateSite(
        [FromRoute] Guid id,
        [FromBody] CreateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateSiteCommand(id, request.Name), cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }
    
    [Authorize(Policy = ScopeConstants.ScopeManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:guid}/delete")]
    public async Task<IActionResult> DeleteSite(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteSiteCommand(id), cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }
}
