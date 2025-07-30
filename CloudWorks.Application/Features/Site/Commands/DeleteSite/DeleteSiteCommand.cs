using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;

namespace CloudWorks.Application.Features.Site.Commands.DeleteSite;

public record DeleteSiteCommand(Guid Id): ICommand<Response<string>>;