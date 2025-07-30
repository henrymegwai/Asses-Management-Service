using FluentValidation;

namespace CloudWorks.Application.Features.Booking.Commands.UpdateBooking;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Booking Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Booking Id cannot be an empty GUID.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("SiteId is required.");
    }
}