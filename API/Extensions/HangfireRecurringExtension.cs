using FidoDino.Application.Services;
using FidoDino.Domain.Enums.Game;
using Hangfire;

namespace FidoDino.API.Extensions;

public static class HangfireRecurringExtension
{
    public static void UseLeaderboardSummaryJobs(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var configValue = configuration["Leaderboard:DefaultTimeRange"];
        if (!Enum.TryParse<TimeRangeType>(configValue, true, out var timeRange))
            timeRange = TimeRangeType.Day;

        var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        switch (timeRange)
        {
            case TimeRangeType.Day:
                manager.AddOrUpdate<LeaderboardSummaryService>(
                    "summary-day",
                    s => s.SummarizeAndResetAsync(TimeRangeType.Day, 100),
                    Cron.Daily(23, 59));
                break;

            case TimeRangeType.Week:
                manager.AddOrUpdate<LeaderboardSummaryService>(
                    "summary-week",
                    s => s.SummarizeAndResetAsync(TimeRangeType.Week, 100),
                    Cron.Weekly(DayOfWeek.Sunday, 23, 59));
                break;

            case TimeRangeType.Month:
                manager.AddOrUpdate<LeaderboardSummaryService>(
                    "summary-month",
                    s => s.SummarizeAndResetAsync(TimeRangeType.Month, 100),
                    Cron.Monthly(23, 59));
                break;
        }
    }
}
