using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.OpenAccessPoint.Commands;

public class OpenAccessPointValidator : AbstractValidator<OpenAccessPointCommand>
{
    public OpenAccessPointValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("Site ID is required.");

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithMessage("Profile ID is required.");

        RuleFor(x => x.AccessPointId)
            .NotEmpty()
            .WithMessage("Access Point ID is required.");
    }
}