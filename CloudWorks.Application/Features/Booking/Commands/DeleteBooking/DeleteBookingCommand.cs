using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;

namespace CloudWorks.Application.Features.Booking.Commands.DeleteBooking;

public record DeleteBookingCommand(Guid Id) : ICommand<Response<string>>;