using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.FreeTimeSlot;

public class GetFreeTimeSlotsValidator: AbstractValidator<GetFreeTimeSlotsQuery>
{
    public GetFreeTimeSlotsValidator()
    {
        RuleFor(x => x.AccessPointId)
            .NotEmpty().WithMessage("Access Point ID is required.");

        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From date is required.");

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("To date is required.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero.");
    }
}