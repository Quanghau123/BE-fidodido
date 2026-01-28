using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Entities.System;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface ILeaderboardService
    {
        Task UpdateUserScoreAsync(
            LeaderboardState state,
            TimeRangeType timeRange,
            DateTime date);
        Task<IEnumerable<LeaderboardUserDto>> GetTopAsync(TimeRangeType timeRange, DateTime date, int count);
        Task<int> GetUserRankAsync(Guid userId, TimeRangeType timeRange, DateTime date);
    }
}