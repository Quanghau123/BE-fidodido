using FidoDino.Application.Interfaces;
using FidoDino.Application.Services;
using FidoDino.Infrastructure.Auth;

namespace FidoDino.API.Extensions;

public static class OAuthExtension
{
    public static IServiceCollection AddOAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GoogleOAuthOptions>(
            configuration.GetSection("Google"));

        services.Configure<FacebookOAuthOptions>(
            configuration.GetSection("Facebook"));

        services.AddHttpClient();
        services.AddScoped<IOAuthClient, GoogleOAuthClient>();
        services.AddScoped<IOAuthClient, FacebookOAuthClient>();
        services.AddScoped<IOAuthService, OAuthService>();

        return services;
    }
}
