using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Site.Commands.DeleteSite;

public class DeleteSiteCommandHandler(
    IRepository<Data.Entities.Site> repository,
    ILogger<DeleteSiteCommandHandler> logger,
    IValidator<DeleteSiteCommand> validator)
    : ICommandHandler<DeleteSiteCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(DeleteSiteCommand),
                validationResult.Errors);
            return new Response<string>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var siteExist = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (siteExist is { IsSuccess: true, Value: null })
        {
            return new Response<string>(false, null!, "Site not found.");
        }
        else if(siteExist.IsFailed)
        {
            logger.LogError("Failed to retrieve site: {Errors}", siteExist.Errors);
            return new Response<string>(false, null!, "Failed to retrieve site.");
        }
        
        var result = await repository.DeleteAsync(x => x.Id == request.Id, cancellationToken);
        return result.IsSuccess ? new Response<string>(true, null!, "Site deleted successfully.")
            : new Response<string>(false, null!, "Site deletion failed.");
    }
}