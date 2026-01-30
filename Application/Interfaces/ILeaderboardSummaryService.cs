using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface ILeaderboardSummaryService
    {
        Task SummarizeAndResetAsync(TimeRangeType timeRange, int topN);
        Task<List<(LeaderboardSnapshot Snapshot, string UserName)>> GetSnapshotsAsync(TimeRangeType timeRange, DateTime forDate, int topN);
    }
}