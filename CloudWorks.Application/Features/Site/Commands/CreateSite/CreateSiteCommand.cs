using CloudWorks.Application.Common.Interfaces.ICommand;
using CloudWorks.Application.Common.Models;
using CloudWorks.Application.Dtos;
using CloudWorks.Data.Entities;

namespace CloudWorks.Application.Features.Site.Commands.CreateSite;

public record CreateSiteCommand(string Name) : ICommand<Response<SiteDto>>;