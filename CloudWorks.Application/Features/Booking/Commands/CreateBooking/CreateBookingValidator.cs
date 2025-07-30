using FluentValidation;

namespace CloudWorks.Application.Features.Booking.Commands.CreateBooking;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Site ID must not be an empty GUID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.UsersEmails)
            .NotEmpty().WithMessage("At least one user email is required.")
            .Must(emails => emails.Count > 0).WithMessage("User emails must not be empty.")
            .Must(emails => emails.All(email => !string.IsNullOrWhiteSpace(email)))
            .WithMessage("User emails must not contain empty strings.")
            .Must(emails => emails.Distinct().Count() == emails.Count)
            .WithMessage("User emails must be unique.")
            .ForEach(email => email
                .NotEmpty().WithMessage("User email must not be empty.")
                .EmailAddress().WithMessage("User email must be a valid email address."));

        RuleFor(x => x.AccessPoints)
            .NotEmpty().WithMessage("At least one access point is required.")
            .Must(accessPoints => accessPoints.Count > 0).WithMessage("Access points must not be empty.")
            .ForEach(accessPoint => accessPoint
                .NotEmpty().WithMessage("Access point must not be empty.")
                .Must(ap => ap != Guid.Empty).WithMessage("Access point must not be an empty GUID."));
        
        RuleFor(x => x.Schedules)
            .NotEmpty().WithMessage("At least one schedule is required.")
            .Must(schedules => schedules.Count > 0).WithMessage("Schedules must not be empty.")
            .ForEach(sch => sch
                .NotEmpty().WithMessage("Schedule must not be empty.")
                .Must(s => s.Start != default && s.End != default)
                .WithMessage("Schedule start and end times must be specified.")
                .Must((command, schedule) => schedule.Start < schedule.End)
                .WithMessage("Schedule end time must be after the start time."));
    }
}