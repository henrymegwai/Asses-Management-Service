using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Mapper;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Infrastructure.Common.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;

public class UpdateAccessPointCommandHandler(
    IRepository<Data.Entities.AccessPoint> repository,
    IValidator<UpdateAccessPointCommand> validator,
    ILogger<UpdateAccessPointCommandHandler> logger)
    : ICommandHandler<UpdateAccessPointCommand, Response<AccessPointDto>>
{
    public async Task<Response<AccessPointDto>> Handle(UpdateAccessPointCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {Command}: {Errors}", nameof(UpdateAccessPointCommand),
                validationResult.Errors);
            return new Response<AccessPointDto>(false, null!, "Validation failed.",
                Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        
        var accessPointExist = await repository.GetByIdAsync(request.Id, cancellationToken);
       
        if (accessPointExist.IsFailed || accessPointExist.Value == null)
        {
            return new Response<AccessPointDto>(false, null!, "Access point not found.");
        }

        var entity = accessPointExist.Value;
        entity.Name = request.Name;
        entity.SiteId = request.SiteId; 
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = Guid.NewGuid();
        var updateExist = await repository.UpdateAsync(entity, cancellationToken);
        
        if (updateExist.IsFailed)
        {
            logger.LogError("Failed to update access point {Id}: {Errors}", request.Id, updateExist.Errors);
            return new Response<AccessPointDto>(false, null!, "Failed to update access point.",
                Result.Fail(updateExist.Errors.Select(x => x.Message).ToList()));
        }
        
        logger.LogInformation("Access point {Id} updated successfully.", request.Id);
        return new Response<AccessPointDto>(true, entity.MapToDto(), "Access point updated successfully.");
    }
}