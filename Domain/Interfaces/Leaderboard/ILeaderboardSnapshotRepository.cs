using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Interfaces.Leaderboard
{
    public interface ILeaderboardSnapshotRepository
    {
        Task<IEnumerable<LeaderboardSnapshot>> GetByTimeRangeAsync(TimeRangeType timeRange, string timeKey);
        Task AddAsync(LeaderboardSnapshot snapshot);
    }
}