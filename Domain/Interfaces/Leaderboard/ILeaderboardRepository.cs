namespace FidoDino.Domain.Interfaces.Leaderboard
{
    public interface ILeaderboardRepository
    {
        Task<IEnumerable<(Guid userId, int score)>> GetTopAsync(string timeRange, int count);
        Task<int> GetUserRankAsync(Guid userId, string timeRange);
        Task AddOrUpdateScoreAsync(Guid userId, int score, string timeRange);
    }
}
