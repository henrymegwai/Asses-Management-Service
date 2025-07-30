using Ical.Net.DataTypes;

namespace CloudWorks.Application.Dtos;

public class TimeSlotDto
{
    public CalDateTime Start { get; set; } = null!;
    public CalDateTime End { get; set; } = null!;
}