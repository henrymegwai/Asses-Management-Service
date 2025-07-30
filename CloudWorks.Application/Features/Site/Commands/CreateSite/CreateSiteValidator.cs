using FluentValidation;

namespace CloudWorks.Application.Features.Site.Commands.CreateSite;

public class CreateSiteValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Site name is required.")
            .MaximumLength(100)
            .WithMessage("Site name must not exceed 100 characters.");
    }
}