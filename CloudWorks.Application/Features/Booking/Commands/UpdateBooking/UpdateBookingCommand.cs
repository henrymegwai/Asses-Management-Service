using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Booking.Commands.UpdateBooking;

public record UpdateBookingCommand(Guid Id,
    string Name,
    Guid SiteId) : ICommand<Response<BookingDto>>;