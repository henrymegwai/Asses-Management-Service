using CloudWorks.Api.Endpoints.Requests;
using CloudWorks.Application.Common.Authentication;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;
using CloudWorks.Application.Features.AccessPoint.Commands.OpenAccessPoint.Commands;
using CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;
using CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudWorks.Api.Endpoints;

[ApiController]
[Route("accesspoints")]
public class AccessPointsController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{siteId:guid}")]
    public async Task<IActionResult> Get([FromRoute]Guid siteId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccessPointsBySiteIdQuery(siteId), cancellationToken);
        return result.Status ? Ok(result) : BadRequest(result.Error);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("histories/{siteId:guid}")]
    public async Task<ActionResult<List<AccessPointHistoryDto>>> GetAccessPointHistories(
        [FromRoute] Guid siteId,
        [FromQuery] string? name,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] bool desc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetAccessPointHistoriesQuery(siteId, new QueryParamsModel(name, sortBy, desc, page, pageSize),
                start, end), cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result.Error);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id}/free-time-slots")]
    public async Task<IActionResult> GetFreeTimeSlots(
        Guid id, DateTime startTime, DateTime endTime, int durationMinutes,
        CancellationToken cancellationToken)
    {
        var query = new GetFreeTimeSlotsQuery(id, startTime, endTime, durationMinutes);
        var result = await mediator.Send(query, cancellationToken);
        if (result.Status)
            return Ok(result);
        return BadRequest(result);
    }
  
    [Authorize(Policy = ScopeConstants.ScopeManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccessPoint(
        [FromBody] CreateAccessPointRequest request,
        CancellationToken cancellationToken)
    {
        var result 
            = await mediator.Send( new CreateAccessPointCommand(request.Name, request.SiteId), cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }
    
    [Authorize(Policy = ScopeConstants.ScopeManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}/update")]
    public async Task<IActionResult> UpdateAccessPoint(
        [FromRoute] Guid id,
        [FromBody] UpdateAccessPointRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateAccessPointCommand(id, request.Name, request.SiteId), cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result.Error);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("{id:guid}/open")]
    public async Task<IActionResult> OpenAccessPoint(
        [FromRoute] Guid id,
        [FromBody] OpenAccessPointRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new OpenAccessPointCommand(id, request.ProfileId, request.SiteId), cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result.Error);
    }
    
    [Authorize(Policy = ScopeConstants.ScopeManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccessPoint(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteAccessPointCommand(id), cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result.Error);
    }
}
