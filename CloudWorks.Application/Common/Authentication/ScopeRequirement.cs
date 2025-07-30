using Microsoft.AspNetCore.Authorization;

namespace CloudWorks.Application.Common.Authentication;

public class ScopeRequirement(string scope) : IAuthorizationRequirement
{
    public string Scope { get; } = scope;
}