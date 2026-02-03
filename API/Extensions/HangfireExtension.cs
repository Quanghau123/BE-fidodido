using Hangfire;
using Hangfire.PostgreSql;

namespace FidoDino.API.Extensions;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfireJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
#pragma warning disable CS0618 
        services.AddHangfire(x =>
            x.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
#pragma warning restore CS0618 
        services.AddHangfireServer();
        return services;
    }
}
