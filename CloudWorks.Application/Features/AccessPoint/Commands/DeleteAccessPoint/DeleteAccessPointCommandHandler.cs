using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;

namespace CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;

public class DeleteAccessPointCommandHandler(
    IRepository<Data.Entities.AccessPoint> repository,
    IValidator<DeleteAccessPointCommand> validator)
    : ICommandHandler<DeleteAccessPointCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteAccessPointCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new Response<string>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }

        var accessPoint 
            = await repository.GetByAsync(x => x.Id == request.Id, cancellationToken);
        
        if (accessPoint is { IsSuccess: true, Value: null })
        {
            return new Response<string>(false, null!, "Access Point not found.");
        }
        else if(accessPoint.IsFailed)
        {
            return new Response<string>(false, null!, "An error occurred while checking for the access point.");
        }

        var result = await repository.DeleteAsync(x => x.Id == request.Id, cancellationToken);
        return result.IsSuccess
            ? new Response<string>(true, null!, "Access Point deleted successfully.")
            : new Response<string>(false, null!, "Access Point deletion failed.");
    }
}