using FluentValidation;

namespace CloudWorks.Application.Features.Site.Commands.DeleteSite;

public class DeleteSiteValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Site ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Site ID cannot be an empty GUID.");
    }
}