using CloudWorks.Api.Middlewares;
using CloudWorks.Application.Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace CloudWorks.Api.Configuration;

public static class ConfigurationExtensions
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        ILogger logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(ConfigurationExtensions));
        var scopes = configuration.GetValue<string>("IdentityServer:Scope")!;
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = configuration.GetValue<string>("IdentityServer:Authority");
                options.Audience = configuration.GetValue<string>("IdentityServer:Audience");
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {

                        var claims = context.Principal!.Claims.ToList();
                        var scope = claims.FirstOrDefault(c => c.Type == "scope" && c.Value == scopes)?.Value;
                        if (scope == null)
                        {
                            context.Fail("The scope claim does not match the required scope");
                            logger.LogInformation("User authenticated failed with scope: {scope}", scope);
                        }
                        else
                        {
                            logger.LogInformation("User authenticated with scope: {scope}", scope);
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        logger.LogError(context.Exception,
                            $"User authentication failed with error: {context.Exception.Message}");

                        throw new Exception(context.Exception.Message);
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            logger.LogError(
                                $"User authentication failed with error: {context.AuthenticateFailure.Message}");
                            return context.Response.WriteAsync("Authentication.Failed");
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        logger.LogError($"User forbidden with error: You do not have access to this resource");

                        throw new Exception($"User forbidden with error: You do not have access to this resource");
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(ScopeConstants.ScopeUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new ScopeRequirement(scopes));
            })
            .AddPolicy(ScopeConstants.ScopeManage, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new ScopeRequirement(scopes));
            });
        services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();
    }
}
