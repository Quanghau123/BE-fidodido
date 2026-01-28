using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface ILeaderboardAppService
    {
        Task<LeaderboardState?> GetUserLeaderboardState(Guid userId, TimeRangeType timeRange, DateTime date);
        Task AddOrUpdateLeaderboardState(LeaderboardState state);
    }
}