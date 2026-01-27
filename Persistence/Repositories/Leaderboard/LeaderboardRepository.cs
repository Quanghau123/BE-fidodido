using FidoDino.Common;
using StackExchange.Redis;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Infrastructure.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly IDatabase _redis;

        public LeaderboardRepository(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        private static string BuildKey(TimeRangeType timeRange, DateTime date)
        {
            var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(timeRange, date);
            return $"leaderboard:{timeRange}:{timeKey}";
        }

        public async Task UpdateScoreAsync(
            TimeRangeType timeRange,
            DateTime date,
            Guid userId,
            long compositeScore,
            int realScore)
        {
            var key = BuildKey(timeRange, date);
            await _redis.SortedSetAddAsync(key, userId.ToString(), compositeScore);
            await _redis.HashSetAsync($"{key}:scores", userId.ToString(), realScore);
        }

        public async Task<IEnumerable<(Guid userId, long compositeScore, int realScore)>> GetTopAsync(TimeRangeType timeRange, DateTime date, int count)
        {
            var key = BuildKey(timeRange, date);
            var entries = await _redis.SortedSetRangeByRankWithScoresAsync(key, 0, count - 1, Order.Descending);
            var userIds = entries.Select(e => e.Element!.ToString()).ToArray();
            var realScores = userIds.Length > 0
                ? (await _redis.HashGetAsync($"{key}:scores", userIds.Select(x => (RedisValue)x).ToArray()))
                : Array.Empty<RedisValue>();
            var result = new List<(Guid userId, long compositeScore, int realScore)>();
            for (int i = 0; i < entries.Length; i++)
            {
                var userId = Guid.Parse(entries[i].Element!);
                var compositeScore = (long)entries[i].Score;
                int realScore = 0;
                if (realScores.Length > i && realScores[i].HasValue && int.TryParse(realScores[i], out var parsedScore))
                    realScore = parsedScore;
                result.Add((userId, compositeScore, realScore));
            }
            return result;
        }

        public async Task<int?> GetUserRankAsync(
            TimeRangeType timeRange,
            DateTime date,
            Guid userId)
        {
            var key = BuildKey(timeRange, date);
            var rank = await _redis.SortedSetRankAsync(
                key,
                userId.ToString(),
                Order.Descending);
            return rank.HasValue ? (int?)rank.Value + 1 : null;
        }

        public async Task ResetAsync(TimeRangeType timeRange, DateTime date)
        {
            var key = BuildKey(timeRange, date);
            await _redis.KeyDeleteAsync(key);
            await _redis.KeyDeleteAsync($"{key}:scores");
        }
    }
}
