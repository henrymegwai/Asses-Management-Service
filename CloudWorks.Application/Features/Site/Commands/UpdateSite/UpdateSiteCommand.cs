using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;

namespace CloudWorks.Application.Features.Site.Commands.UpdateSite;

public record UpdateSiteCommand(Guid Id, string Name) : ICommand<Response<SiteDto>>;