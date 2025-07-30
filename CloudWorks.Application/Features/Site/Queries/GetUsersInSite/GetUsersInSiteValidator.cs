using FluentValidation;

namespace CloudWorks.Application.Features.Site.Queries.GetUsersInSite;

public class GetUsersInSiteValidator : AbstractValidator<GetUsersInSiteQuery>
{
    public GetUsersInSiteValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID must not be empty.")
            .NotNull().WithMessage("Site ID must not be null.");
    }
}