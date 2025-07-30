using CloudWorks.Application.Common.Models;

namespace CloudWorks.Application.Dtos;

public record BookingDto(
    string Name,
    List<string> UserEmails,
    List<Guid> AccessPoints,
    List<ScheduleModel> Schedules);