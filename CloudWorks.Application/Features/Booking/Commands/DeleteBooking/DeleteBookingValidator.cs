using FluentValidation;

namespace CloudWorks.Application.Features.Booking.Commands.DeleteBooking;

public class DeleteBookingValidator : AbstractValidator<DeleteBookingCommand>
{
    public DeleteBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Booking ID is required.")
            .NotNull().WithMessage("Booking ID cannot be null.");
    }
}