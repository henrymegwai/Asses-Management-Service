using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Booking.Commands.UpdateBooking;

public class UpdateBookingCommandHandler(
    IRepository<Data.Entities.Booking> bookingRepository,
    IValidator<UpdateBookingCommand> validator,
    ILogger<UpdateBookingCommandHandler> logger)
    : ICommandHandler<UpdateBookingCommand, Response<BookingDto>>
{
    public async Task<Response<BookingDto>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(UpdateBookingCommand),
                validationResult.Errors);
            return new Response<BookingDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var existingBooking = bookingRepository.GetByIdAsync(request.Id, cancellationToken).Result;
        if (existingBooking.IsFailed || existingBooking.Value == null)
        {
            logger.LogWarning("Booking with Id {Id} not found.", request.Id);
            return new Response<BookingDto>(false, null!, $"Booking with Id {request.Id} not found.",
                Result.Fail("Booking not found."));
        }

        var entity = UpdateBookingEntity(existingBooking.Value, request);

        var result = await bookingRepository.UpdateAsync(entity, cancellationToken);

        if (result.IsSuccess) return new Response<BookingDto>(true, entity.MapToDto(), 
            "Booking updated successfully.");

        logger.LogError("Failed to update booking with Id {Id}: {Errors}", request.Id, result.Errors);
        return new Response<BookingDto>(false, null!, "Failed to update booking.",
            Result.Fail(result.Errors));
    }
    
    private Data.Entities.Booking UpdateBookingEntity(Data.Entities.Booking entity, UpdateBookingCommand request)
    {
        entity.Name = request.Name;
        entity.SiteId = request.SiteId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = Guid.NewGuid();
        return entity;
    }
}