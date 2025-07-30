using CloudWorks.Application.Common.Interfaces.IQuery;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Common.Utilities;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Booking.Queries.GetBooking;

public record GetBookingsQuery(QueryParamsModel QueryParams) : IQuery<Response<Page<BookingDto>>>;