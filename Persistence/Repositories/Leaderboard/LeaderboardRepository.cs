using FidoDino.Domain.Interfaces.Leaderboard;
using StackExchange.Redis;

namespace FidoDino.Persistence.Repositories.Leaderboard
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IDatabase _redis;
        public LeaderboardRepository(IConnectionMultiplexer redisConnection)
        {
            _redis = redisConnection.GetDatabase();
        }
        /// <summary>
        /// Hàm tạo key cho bảng xếp hạng dựa trên khoảng thời gian (ngày, tuần, tháng).
        /// </summary>
        private string GetLeaderboardKey(string timeRange)
        {
            var now = DateTime.UtcNow;
            return timeRange.ToLower() switch
            {
                "day" => $"leaderboard:day:{now:yyyy-MM-dd}",
                "week" => $"leaderboard:week:{now:yyyy-ww}",
                "month" => $"leaderboard:month:{now:yyyy-MM}",
                _ => $"leaderboard:{timeRange}:{now:yyyy-MM-dd}"
            };
        }

        /// <summary>
        /// Lấy danh sách top (count) người chơi có điểm cao nhất theo khoảng thời gian.
        /// </summary>
        public async Task<IEnumerable<(Guid userId, int score)>> GetTopAsync(string timeRange, int count)
        {
            var key = GetLeaderboardKey(timeRange);
            var entries = await _redis.SortedSetRangeByScoreWithScoresAsync(key, order: Order.Descending, take: count);
            var result = new List<(Guid, int)>();
            foreach (var entry in entries)
            {
                if (Guid.TryParse(entry.Element, out var userId))
                    result.Add((userId, (int)entry.Score));
            }
            return result;
        }
        /// <summary>
        /// Lấy thứ hạng của một người chơi trong bảng xếp hạng theo khoảng thời gian.
        /// </summary>
        public async Task<int> GetUserRankAsync(Guid userId, string timeRange)
        {
            var key = GetLeaderboardKey(timeRange);
            var rank = await _redis.SortedSetRankAsync(key, userId.ToString(), Order.Descending);
            return rank.HasValue ? (int)rank.Value + 1 : -1;
        }
        /// <summary>
        /// Thêm mới hoặc cập nhật điểm số của người chơi trong bảng xếp hạng theo khoảng thời gian.
        /// </summary>
        public async Task AddOrUpdateScoreAsync(Guid userId, int score, string timeRange)
        {
            var key = GetLeaderboardKey(timeRange);
            await _redis.SortedSetAddAsync(key, userId.ToString(), score);
        }
    }
}
