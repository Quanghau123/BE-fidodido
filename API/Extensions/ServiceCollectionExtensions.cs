using FidoDino.Application.Events;
using FidoDino.Application.Interfaces;
using FidoDino.Application.Services;
using FidoDino.Common.Authorization;
using FidoDino.Infrastructure.Realtime;
using FidoDino.Infrastructure.Realtime.Handlers;
using FidoDino.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace FidoDino.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Auth / User
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        // Game
        services.AddScoped<IGamePlayService, GamePlayService>();
        services.AddScoped<IGameSessionService, GameSessionService>();
        services.AddScoped<IPlayTurnService, PlayTurnService>();
        services.AddScoped<IEffectService, EffectService>();
        services.AddScoped<IItemService, ItemService>();

        // Leaderboard
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<ILeaderboardAppService, LeaderboardAppService>();
        services.AddScoped<ISnapshotService, SnapshotService>();
        services.AddScoped<ILeaderboardSummaryService, LeaderboardSummaryService>();

        // System
        services.AddScoped<IStartupService, StartupService>();
        services.AddScoped<ISystemStatusAppService, SystemStatusAppService>();

        // EventBus
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        // handler realtime 
        services.AddScoped<LeaderboardRealtimeHandler>();
        return services;
    }
}
