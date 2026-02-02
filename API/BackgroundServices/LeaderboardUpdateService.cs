// using Microsoft.AspNetCore.SignalR;
// using FidoDino.Application.Interfaces;
// using FidoDino.Domain.Enums.Game;
// //Hub trong là cầu nối realtime để server push dữ liệu trực tiếp tới client.
// using FidoDino.API.Hubs;

// namespace FidoDino.API.BackgroundServices
// {
//     //BackgroundService: Singleton sống suốt vòng đời ứng dụng
//     public class LeaderboardUpdateService : BackgroundService
//     {
//         private readonly IServiceProvider _serviceProvider;
//         private readonly IHubContext<LeaderboardHub> _hubContext;
//         private readonly TimeRangeType _defaultTimeRange;
//         private readonly int _topUsers;

//         public LeaderboardUpdateService(IServiceProvider serviceProvider, IHubContext<LeaderboardHub> hubContext, IConfiguration config)
//         {
//             _serviceProvider = serviceProvider;
//             _hubContext = hubContext;
//             Enum.TryParse(config["Leaderboard:DefaultTimeRange"], true, out _defaultTimeRange);
//             if (_defaultTimeRange == 0)
//                 _defaultTimeRange = TimeRangeType.Day;
//             _topUsers = config.GetSection("Leaderboard:TopUsers").Get<int>();
//             if (_topUsers <= 0) _topUsers = 10;
//         }

//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 //ILeaderboardService : Scoped service, cần tạo scope mới để sử dụng trong BackgroundService
//                 using (var scope = _serviceProvider.CreateScope())
//                 {
//                     var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
//                     var timeRange = _defaultTimeRange;
//                     var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
//                     var result = await leaderboardService.GetTopAsync(timeRange, now, _topUsers);
//                     await _hubContext.Clients.All.SendAsync("LeaderboardUpdated", result);
//                     Console.WriteLine($"[DEBUG] LeaderboardUpdateService: Sent leaderboard update at {now} for {timeRange}");
//                 }
//                 await Task.Delay(5000, stoppingToken);
//             }
//         }
//     }
// }