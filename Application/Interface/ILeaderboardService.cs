using FidoDino.Domain.Entities.System;

namespace FidoDino.Application.Interfaces
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<(Guid userId, int score)>> GetTopAsync(string timeRange, int count);
        Task<int> GetUserRankAsync(Guid userId, string timeRange);
    }
}