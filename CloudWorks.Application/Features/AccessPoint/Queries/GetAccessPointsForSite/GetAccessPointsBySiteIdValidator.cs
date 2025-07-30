using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointsForSite;

public class GetAccessPointsBySiteIdValidator : AbstractValidator<GetAccessPointsBySiteIdQuery>
{
    public GetAccessPointsBySiteIdValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("SiteId is required.");
    }
}