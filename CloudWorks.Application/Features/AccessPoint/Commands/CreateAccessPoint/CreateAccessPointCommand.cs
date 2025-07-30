using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.AccessPoint.Commands.CreateAccessPoint;

public record CreateAccessPointCommand(string Name, Guid SiteId) : ICommand<Response<AccessPointDto>>;