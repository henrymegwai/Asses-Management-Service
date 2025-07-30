using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.AccessPoint.Commands.UpdateAccessPoint;

public record UpdateAccessPointCommand(Guid Id, string Name, Guid SiteId) : ICommand<Response<AccessPointDto>>;