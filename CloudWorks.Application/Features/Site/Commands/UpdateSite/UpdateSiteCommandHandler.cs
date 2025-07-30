using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Site.Commands.UpdateSite;

public class UpdateSiteCommandHandler(
    IRepository<Data.Entities.Site> repository,
    IValidator<UpdateSiteCommand> validator,
    ILogger<UpdateSiteCommandHandler> logger)
    : ICommandHandler<UpdateSiteCommand, Response<SiteDto>>
{
    public async Task<Response<SiteDto>> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(UpdateSiteCommand),
                validationResult.Errors);
            return new Response<SiteDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var siteExist = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (siteExist.IsFailed)
        {
            logger.LogWarning("Site with ID {Id} not found.", request.Id);
            return new Response<SiteDto>(false, null!, $"Site with ID {request.Id} not found.");
        }
        
        var response = await UpdateSiteEntityAsync(siteExist.Value, request, cancellationToken);
        return response;
    }
    
    private async Task<Response<SiteDto>> UpdateSiteEntityAsync(
        Data.Entities.Site entity, 
        UpdateSiteCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.Name == entity.Name)
        {
            logger.LogInformation("No changes detected for site {Id}.", request.Id);
            return new Response<SiteDto>(true, entity.MapToDto(), "No changes detected.");
        }

        entity.Name = request.Name;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = Guid.NewGuid();
        var updateResult = await repository.UpdateAsync(entity, cancellationToken);

        if (updateResult.IsFailed)
        {
            logger.LogError("Failed to update site {Id}: {Errors}", request.Id, updateResult.Errors);
            return new Response<SiteDto>(false, null!, "Failed to update site.",
                Result.Fail(updateResult.Errors.Select(x => x.Message).ToList()));
        }

        logger.LogInformation("Site {Id} updated successfully.", request.Id);
        return new Response<SiteDto>(true, entity.MapToDto(), "Site updated successfully.");
    }
}