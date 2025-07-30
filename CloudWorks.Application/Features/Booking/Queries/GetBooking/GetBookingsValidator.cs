using FluentValidation;

namespace CloudWorks.Application.Features.Booking.Queries.GetBooking;

public class GetBookingsValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsValidator()
    {
        RuleFor(x => x.QueryParams.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.QueryParams.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.QueryParams.SortBy)
            .Must(sortBy => sortBy == "Name" || sortBy == "Date")
            .WithMessage("Sort by must be either 'Name' or 'Date'.");

        RuleFor(x => x.QueryParams.Desc)
            .NotNull()
            .WithMessage("Descending order must be specified.");
    }
}