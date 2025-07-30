using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;

public class UpdateAccessPointValidator : AbstractValidator<UpdateAccessPointCommand>
{
    public UpdateAccessPointValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Access point ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Access point name is required.")
            .MaximumLength(50)
            .WithMessage("Access point name must not exceed 50 characters.");

        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("Site ID is required.");
    }
}