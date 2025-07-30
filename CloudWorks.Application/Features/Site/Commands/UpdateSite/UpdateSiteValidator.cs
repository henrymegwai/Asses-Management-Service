using FluentValidation;

namespace CloudWorks.Application.Features.Site.Commands.UpdateSite;

public class UpdateSiteValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}