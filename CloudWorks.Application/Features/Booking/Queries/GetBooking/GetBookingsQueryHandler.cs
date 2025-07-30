using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Booking.Queries.GetBooking;

public class GetBookingsQueryHandler(
    IRepository<Data.Entities.Booking> bookingRepository,
    IValidator<GetBookingsQuery> validator,
    ILogger<GetBookingsQueryHandler> logger)
    : IQueryHandler<GetBookingsQuery, Response<Page<BookingDto>>>
{
    public async Task<Response<Page<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Query}: {Errors}", nameof(GetBookingsQuery),
                validationResult.Errors);
            return new Response<Page<BookingDto>>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var result = GetSitesResultAsync(request);
        
        return new Response<Page<BookingDto>>(true, result, "Bookings retrieved successfully.");
    }
    
    private Page<BookingDto> GetSitesResultAsync(GetBookingsQuery request)
    {
        // Filtering
        var query = string.IsNullOrEmpty(request.QueryParams.Name)
            ? bookingRepository.Query(null, null, [])
            : bookingRepository.Query(x => x.Name == request.QueryParams.Name, null, []);

        // Sorting
        query = request.QueryParams.SortBy switch
        {
            "Name" => request.QueryParams.Desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            _ => request.QueryParams.Desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name)
        };

        var queryResult = query.Select(s => s.MapToDto());

        // Pagination
        return PageExtension<BookingDto>.ToPageListAsync(queryResult,
            request.QueryParams.PageNumber,
            request.QueryParams.PageSize);
    }
}