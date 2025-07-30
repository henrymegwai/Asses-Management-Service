using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;

public class CreateAccessPointCommandHandler(
    IRepository<Data.Entities.AccessPoint> repository,
    IValidator<CreateAccessPointCommand> validator,
    ILogger<CreateAccessPointCommandHandler> logger)
    : ICommandHandler<CreateAccessPointCommand, Response<AccessPointDto>>
{

    public async Task<Response<AccessPointDto>> Handle(CreateAccessPointCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(CreateAccessPointCommand), validationResult.Errors);
            return new Response<AccessPointDto>(false, null!, "Validation failed.", 
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var existingAccessPoint = await repository.GetByAsync(x => x.Name == request.Name, cancellationToken);

        if (existingAccessPoint is { IsSuccess: true, Value: not null })
        {
            logger.LogWarning("An access point with the name {Name} already exists.", request.Name);
            return new Response<AccessPointDto>(false, null!, "An access point with the same name already exists.");
        }
        else if(existingAccessPoint.IsFailed)
        {
            logger.LogError("Error occurred while checking for existing access point: {Error}", existingAccessPoint.Errors);
            return new Response<AccessPointDto>(false, null!, "An error occurred while checking for existing access points.");
        }
        
        var result = await CreateAndAddAccessPointAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            return new Response<AccessPointDto>(true, result.Value.MapToDto(), "Access point created successfully.");
        }

        logger.LogError("Failed to create access point: {Error}", result.Errors);
        return new Response<AccessPointDto>(false, null!, "Failed to create access point.");
    }
    
    private async Task<Result<Data.Entities.AccessPoint>> CreateAndAddAccessPointAsync(
        CreateAccessPointCommand request, 
        CancellationToken cancellationToken)
    {
        var entity = new Data.Entities.AccessPoint
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SiteId = request.SiteId,
            CreatedAt = DateTime.Now, 
            CreatedBy = Guid.NewGuid()
        };

        return await repository.AddAsync(entity, cancellationToken);
    }
}