using FidoDino.Application.Interfaces;
using FidoDino.Application.Events;
using Microsoft.AspNetCore.SignalR;
using FidoDino.API.Hubs;

namespace FidoDino.Infrastructure.Realtime.Handlers;

public class LeaderboardRealtimeHandler
    : IEventHandler<LeaderboardUpdatedEvent>
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly IHubContext<LeaderboardHub> _hub;

    public LeaderboardRealtimeHandler(
        ILeaderboardService leaderboardService,
        IHubContext<LeaderboardHub> hub)
    {
        _leaderboardService = leaderboardService;
        _hub = hub;
    }

    public async Task HandleAsync(LeaderboardUpdatedEvent evt)
    {
        var leaderboard = await _leaderboardService.GetTopAsync(
            evt.TimeRange,
            evt.Date,
            evt.Top
        );

        var payload = new
        {
            timeRange = evt.TimeRange,         
            date = evt.Date.ToString("yyyyMMdd"),
            items = leaderboard
        };

        await _hub.Clients
            .All
            .SendAsync("leaderboard.updated", payload);

        Console.WriteLine(
            $"[LOG] Broadcast leaderboard.updated {evt.TimeRange} {evt.Date:yyyy-MM-dd}");
    }
}
