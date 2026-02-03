using FidoDino.Infrastructure.Redis;

namespace FidoDino.API.Extensions;

public static class CacheExtension
{
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services)
    {
        services.AddScoped<EffectCacheService>();
        services.AddScoped<IceCacheService>();
        services.AddScoped<IceRewardCacheService>();
        services.AddScoped<RewardCacheService>();
        services.AddScoped<SystemStatusRedisLoader>();

        return services;
    }
}
