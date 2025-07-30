using Ical.Net;
using Ical.Net.DataTypes;

namespace CloudWorks.Application.Common.Utilities;

public static class CalDateTimeNormalizer
{
    public static (CalDateTime? Start, CalDateTime? End) GetScheduleStartEnd(this string scheduleValue)
    {
        if(string.IsNullOrEmpty(scheduleValue))
            return (null, null);
        
        var calendar = Calendar.Load(scheduleValue);
        var calendarEvent = calendar!.Events.FirstOrDefault();
        if (calendarEvent == null) return (null, null);

        return (calendarEvent.Start, calendarEvent.End);
    }
    
    public static CalDateTime ToCalDateTime(this DateTime dateTime)
    {
        return new CalDateTime(dateTime);
    }
}