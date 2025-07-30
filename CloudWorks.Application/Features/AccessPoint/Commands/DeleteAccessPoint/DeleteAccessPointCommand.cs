using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;

namespace CloudWorks.Application.Features.AccessPoint.Commands.DeleteAccessPoint;

public record DeleteAccessPointCommand(Guid Id) : ICommand<Response<string>>; 