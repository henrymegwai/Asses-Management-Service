namespace CloudWorks.Api.Endpoints.Requests;

public class AddBookingRequest
{
    public string Name { get; set; } = null!;
    public List<string> UsersEmails { get; set; } = [];
    public List<Guid> AccessPoints { get; set; } = [];
    public List<ScheduleRequest> Schedules { get; set; } = [];
}
