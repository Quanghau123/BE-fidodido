using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Interfaces.Leaderboard
{
    public interface ILeaderboardStateRepository
    {
        Task<LeaderboardState?> GetByUserAndTimeAsync(Guid userId, TimeRangeType timeRange, string timeKey);
        Task AddOrUpdateAsync(LeaderboardState state);
    }
}