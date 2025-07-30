using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using Ical.Net.DataTypes;

namespace CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;

public record GetFreeTimeSlotsQuery(
    Guid AccessPointId,
    DateTime From,
    DateTime To,
    int DurationMinutes
) : IQuery<Response<List<TimeSlotDto>>>;