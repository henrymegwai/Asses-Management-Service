using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.Site.Commands.CreateSite;

public class CreateSiteCommandHandler(
    IRepository<Data.Entities.Site> repository,
    IValidator<CreateSiteCommand> validator,
    ILogger<CreateSiteCommandHandler> logger)
    : ICommandHandler<CreateSiteCommand, Response<SiteDto>>
{
    public async Task<Response<SiteDto>> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(CreateSiteCommand),
                validationResult.Errors);
            return new Response<SiteDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }

        var siteExists = await repository.GetByAsync(x => x.Name == request.Name, cancellationToken);
        if (siteExists is { IsSuccess: true, Value: not null })
        {
            return new Response<SiteDto>(false, null!, $"Site with name {request.Name} already exists.");
        }
        else if(siteExists.IsFailed)
        {
            logger.LogError("Failed to check if site exists: {Errors}", siteExists.Errors);
            return new Response<SiteDto>(false, null!, "Failed to check if site exists.");
        }

        var addSiteResult = await AddSiteAsync(request, cancellationToken);

        if (addSiteResult.IsSuccess)
            return new Response<SiteDto>(true,
                addSiteResult.Value.MapToDto(), "Site created successfully.");

        logger.LogError("Failed to create site: {Errors}", addSiteResult.Errors);
        return new Response<SiteDto>(false, null!, "Failed to create site.");
    }

    private async Task<Result<Data.Entities.Site>> AddSiteAsync(
        CreateSiteCommand request,
        CancellationToken cancellationToken)
    {
        return await repository.AddAsync(new Data.Entities.Site
        {
            Name = request.Name, 
            CreatedAt = DateTime.Now, 
            CreatedBy = Guid.NewGuid()
        }, cancellationToken);
    }
}