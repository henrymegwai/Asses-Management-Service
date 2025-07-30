using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;

namespace CloudWorks.Application.Features.Booking.Commands.DeleteBooking;

public class DeleteBookingCommandHandler(IRepository<Data.Entities.Booking> repository,
    IValidator<DeleteBookingCommand> validator)
    : ICommandHandler<DeleteBookingCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        // validate result and result is not valid
        if (!validationResult.IsValid)
        {
            return new Response<string>(false, null!, "Validation failed.", 
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var booking = await repository.GetByAsync(x => x.Id == request.Id, cancellationToken);
        if (booking is { IsSuccess: true, Value: null })
        {
            return new Response<string>(false, null!, "Booking not found.");
        }
        else if(booking.IsFailed)
        {
            return new Response<string>(false, null!, "Failed to retrieve booking.");
        }
        
        var result = await repository.DeleteAsync(x => x.Id == request.Id, cancellationToken);
        return result.IsSuccess ? new Response<string>(true, null!, "Booking deleted successfully.")
                : new Response<string>(false, null!, "Booking deletion failed.");
    }
}