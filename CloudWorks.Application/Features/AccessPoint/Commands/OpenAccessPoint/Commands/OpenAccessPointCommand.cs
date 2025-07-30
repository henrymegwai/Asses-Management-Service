using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.AccessPoint.Commands.OpenAccessPoint.Commands;

public record OpenAccessPointCommand(
    Guid AccessPointId,
    Guid SiteId,
    Guid ProfileId
) : ICommand<Response<OpenAccessDto>>;