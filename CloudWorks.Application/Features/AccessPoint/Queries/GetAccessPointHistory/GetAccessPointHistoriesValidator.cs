using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Queries.GetAccessPointHistory;

public class GetAccessPointHistoriesValidator : AbstractValidator<GetAccessPointHistoriesQuery>
{
    public GetAccessPointHistoriesValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage("SiteId is required.")
            .When(x => x.SiteId.HasValue);

        RuleFor(x => x.QueryParamsModel.Name)
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters.");

        RuleFor(x => x.QueryParamsModel.SortBy)
            .MaximumLength(20)
            .WithMessage("SortBy cannot exceed 20 characters.");

        RuleFor(x => x.QueryParamsModel.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.QueryParamsModel.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");
    }
}