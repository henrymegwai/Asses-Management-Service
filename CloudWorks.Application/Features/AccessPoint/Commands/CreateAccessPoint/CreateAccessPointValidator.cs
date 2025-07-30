using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;

public class CreateAccessPointValidator : AbstractValidator<CreateAccessPointCommand>
{
    public CreateAccessPointValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Access point name is required.")
            .MaximumLength(100)
            .WithMessage("Access point name must not exceed 100 characters.");

        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("Site ID is required.");
    }
}