using CloudWorks.Api.Endpoints.Requests;
using CloudWorks.Application.Common.Authentication;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Features.Booking.Commands.CreateBooking;
using CloudWorks.Application.Features.Booking.Commands.DeleteBooking;
using CloudWorks.Application.Features.Booking.Commands.UpdateBooking;
using CloudWorks.Application.Features.Booking.Queries.GetBooking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudWorks.Api.Endpoints;

[ApiController]
[Route("bookings")]
public class BookingsController(IMediator mediator) : ControllerBase
{
    
    // I want to get all bookings with pagination, sorting and filtering
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<IActionResult> GetBookings(
        [FromQuery] string? name,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] bool desc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetBookingsQuery(new QueryParamsModel(name, sortBy, desc, page, pageSize)), 
            cancellationToken);
        
        return result.Status ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = ScopeConstants.ScopeUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("{siteId:guid}/create")]
    public async Task<IActionResult> CreateBooking(
        [FromRoute]Guid siteId,
        [FromBody] AddBookingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateBookingCommand(
            siteId,
            request.Name,
            request.UsersEmails,
            request.AccessPoints,
            request.Schedules.Select(x 
                => new ScheduleModel { Start = x.Start, End = x.End }).ToList()), 
            cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }
    
    [Authorize(Policy = ScopeConstants.ScopeUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}/update")]
    public async Task<IActionResult> UpdateBooking(
        [FromRoute] Guid id,
        [FromBody] UpdateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateBookingCommand(id, request.Name, request.SiteId), 
            cancellationToken);

        return result.Status ? Ok(result) : BadRequest(result);
    }
    
    [Authorize(Policy = ScopeConstants.ScopeUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        var result = await mediator.Send(new DeleteBookingCommand(id), CancellationToken.None);
        return result.Status ? Ok(result) : BadRequest(result);
    }
}
