using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Interfaces.Leaderboard
{
    public interface ILeaderboardRepository
    {
        Task UpdateScoreAsync(
            TimeRangeType timeRange,
            DateTime date,
            Guid userId,
            long compositeScore,
            int realScore);

        Task<IEnumerable<(Guid userId, long compositeScore, int realScore)>> GetTopAsync(
            TimeRangeType timeRange,
            DateTime date,
            int count);

        Task<int?> GetUserRankAsync(
            TimeRangeType timeRange,
            DateTime date,
            Guid userId);

        Task ResetAsync(TimeRangeType timeRange, DateTime date);
    }
}
