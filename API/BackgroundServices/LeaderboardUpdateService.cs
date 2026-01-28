using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Enums.Game;
using System;
using System.Threading;
using System.Threading.Tasks;
using FidoDino.API.Hubs;
using Microsoft.Extensions.DependencyInjection;
using FidoDino.Common;

namespace FidoDino.API.BackgroundServices
{
    public class LeaderboardUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<LeaderboardHub> _hubContext;
        private readonly TimeRangeType _defaultTimeRange;

        public LeaderboardUpdateService(IServiceProvider serviceProvider, IHubContext<LeaderboardHub> hubContext, TimeRangeType defaultTimeRange = TimeRangeType.Day)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _defaultTimeRange = defaultTimeRange;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                    var timeRange = _defaultTimeRange;
                    var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
                    var result = await leaderboardService.GetTopAsync(timeRange, now, 10);
                    await _hubContext.Clients.All.SendAsync("LeaderboardUpdated", result);
                    Console.WriteLine($"[DEBUG] LeaderboardUpdateService: Sent leaderboard update at {now} for {timeRange}");
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}