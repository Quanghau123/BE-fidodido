using FidoDino.Domain.Interfaces.Auth;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Domain.Interfaces.System;
using FidoDino.Infrastructure.Repositories;
using FidoDino.Persistence.Repositories.Auth;
using FidoDino.Persistence.Repositories.Game;
using FidoDino.Persistence.Repositories.Leaderboard;
using FidoDino.Persistence.Repositories.System;

namespace FidoDino.API.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<ILeaderboardStateRepository, LeaderboardStateRepository>();
        services.AddScoped<IPlayTurnRepository, PlayTurnRepository>();
        services.AddScoped<IIceRepository, IceRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IIceRewardRepository, IceRewardRepository>();
        services.AddScoped<IActiveEffectRepository, ActiveEffectRepository>();
        services.AddScoped<ISystemStatusRepository, SystemStatusRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ILeaderboardSnapshotRepository, LeaderboardSnapshotRepository>();

        return services;
    }
}
