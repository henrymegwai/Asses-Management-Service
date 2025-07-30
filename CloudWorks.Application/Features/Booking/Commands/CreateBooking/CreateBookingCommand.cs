using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Booking.Commands.CreateBooking;

public record CreateBookingCommand(Guid SiteId,
    string Name,
    List<string> UsersEmails,
    List<Guid> AccessPoints,
    List<ScheduleModel> Schedules) : ICommand<Response<BookingDto>>;