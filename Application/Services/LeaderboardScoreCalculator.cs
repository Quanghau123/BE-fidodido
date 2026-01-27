using FidoDino.Domain.Entities.Leaderboard;

namespace FidoDino.Application.Services
{
    public static class LeaderboardScoreCalculator
    {
        public static long Calculate(LeaderboardState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var achievedUtc = state.AchievedAt.Kind == DateTimeKind.Utc
                ? state.AchievedAt
                : state.AchievedAt.ToUniversalTime();

            var unixTime = new DateTimeOffset(achievedUtc).ToUnixTimeSeconds();

            return (state.TotalScore * 10_000_000L)
                 - (unixTime / 10)
                 - (state.PlayCount * 10)
                 - state.StableRandom;
        }
    }
}
