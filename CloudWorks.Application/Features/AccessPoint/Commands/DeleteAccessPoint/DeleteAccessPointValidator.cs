using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;

public class DeleteAccessPointValidator : AbstractValidator<DeleteAccessPointCommand>
{
    public DeleteAccessPointValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Access Point ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Access Point ID must not be an empty GUID.");
    }
}