using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using FidoDino.Infrastructure.Data;
using FidoDino.Infrastructure.Redis;

namespace FidoDino.API.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FidoDinoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IDatabase>(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        services.AddSingleton<RedisConnectionFactory>(sp =>
            new RedisConnectionFactory(configuration.GetConnectionString("Redis")!));

        return services;
    }
}
