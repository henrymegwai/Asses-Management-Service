using CloudWorks.Application.Common.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CloudWorks.Api.Middlewares;

public class ScopeAuthorizationHandler(ILogger<ScopeAuthorizationHandler> logger, IConfiguration configuration)
    : AuthorizationHandler<ScopeRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "scope" && c.Value.Split(' ').Contains(requirement.Scope)))
        {
            context.Succeed(requirement);
        }
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            logger.LogWarning("Authorization failed for anonymous user to access the endpoint. Required scope: {Scope}", requirement.Scope);
            throw new Exception("User is not authenticated");
        }

        return Task.CompletedTask;
    }

}