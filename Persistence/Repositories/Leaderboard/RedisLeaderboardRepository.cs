using StackExchange.Redis;

namespace FidoDino.Persistence.Repositories.Leaderboard
{
    public class RedisLeaderboardRepository
    {
        private readonly IDatabase _redis;
        public RedisLeaderboardRepository(IDatabase redis)
        {
            _redis = redis;
        }
        /// <summary>
        /// Cập nhật điểm số của người chơi trong bảng xếp hạng Redis.
        /// </summary>
        public async Task UpdateScoreAsync(string leaderboardKey, string userId, double score)
        {
            await _redis.SortedSetAddAsync(leaderboardKey, userId, score);
        }
        /// <summary>
        /// Lấy danh sách top người chơi có điểm cao nhất từ bảng xếp hạng Redis.
        /// </summary>
        public async Task<List<(string userId, double score)>> GetTopAsync(string leaderboardKey, int count)
        {
            var entries = await _redis.SortedSetRangeByScoreWithScoresAsync(leaderboardKey, order: Order.Descending, take: count);
            return entries.Select(e => (e.Element.ToString(), e.Score)).ToList();
        }
        /// <summary>
        /// Lấy thứ hạng của một người chơi trong bảng xếp hạng Redis.
        /// </summary>
        public async Task<long?> GetUserRankAsync(string leaderboardKey, string userId)
        {
            var rank = await _redis.SortedSetRankAsync(leaderboardKey, userId, Order.Descending);
            return rank;
        }
    }
}