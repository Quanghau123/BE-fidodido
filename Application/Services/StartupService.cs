using FidoDino.Application.Interface;
using FidoDino.Common;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FidoDino.Application.Services
{
    public class StartupService : IStartupService
    {
        private readonly FidoDinoDbContext _db;
        private readonly IDatabase _redis;

        public StartupService(FidoDinoDbContext db, IConnectionMultiplexer redis)
        {
            _db = db;
            _redis = redis.GetDatabase();
        }

        /// <summary>
        /// Load dữ liệu nền tảng (người dùng, bảng xếp hạng, phần thưởng, trạng thái hệ thống...) từ database lên Redis.
        /// </summary>
        public async Task LoadPlatformDataToRedisAsync()
        {
            // Load Users
            var users = await _db.Users.AsNoTracking().ToListAsync();
            foreach (var user in users)
            {
                await _redis.HashSetAsync("game:users", user.UserId.ToString(), user.UserName);
            }

            // Load LeaderboardState
            var leaderboardStates = await _db.LeaderboardStates.AsNoTracking().ToListAsync();
            foreach (var state in leaderboardStates)
            {
                var key = $"leaderboard:{state.TimeRange}:{state.TimeKey}";
                await _redis.SortedSetAddAsync(key, state.UserId.ToString(), state.TotalScore);
            }

            // Load Ice
            var ices = await _db.Ices.AsNoTracking().ToListAsync();
            foreach (var ice in ices)
            {
                await _redis.HashSetAsync("game:ice:all", ice.IceId.ToString(), ice.IceType.ToString());
            }

            // Load IceReward
            var iceRewards = await _db.IceRewards.AsNoTracking().ToListAsync();
            foreach (var ice in ices)
            {
                var rewards = iceRewards.Where(r => r.IceId == ice.IceId).Select(r => new { r.RewardId, r.Probability });
                await _redis.StringSetAsync($"game:ice_reward:{ice.IceId}", System.Text.Json.JsonSerializer.Serialize(rewards));
            }

            // Load Reward
            var rewardsList = await _db.Rewards.AsNoTracking().ToListAsync();
            foreach (var reward in rewardsList)
            {
                await _redis.HashSetAsync("game:reward:all", reward.RewardId.ToString(), reward.RewardName);
            }

            // Load SystemStatus
            var systemStatus = await _db.SystemStatuses.OrderByDescending(x => x.UpdatedAt).FirstOrDefaultAsync();
            if (systemStatus != null)
            {
                await _redis.StringSetAsync("game:system_status", systemStatus.StatusCode.ToString());
            }

            // Restore leaderboard from ScoreEvent
            var scoreEvents = await _db.ScoreEvents.Where(e => !e.AppliedToRedis).ToListAsync();
            foreach (var evt in scoreEvents)
            {
                var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(evt.TimeRange, evt.CreatedAt);
                var key = $"leaderboard:{evt.TimeRange}:{timeKey}";
                double score = evt.CompositeDelta ?? evt.ScoreDelta;
                await _redis.SortedSetIncrementAsync(key, evt.UserId.ToString(), score);
                evt.AppliedToRedis = true;
            }
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Kiểm tra trạng thái hệ thống và dữ liệu đã được load lên Redis chưa.
        /// </summary>
        public async Task<object> GetHealthStatusAsync()
        {
            var redisStatus = _redis.Ping();
            var hasIce = await _redis.KeyExistsAsync("game:ice:all");
            var hasReward = await _redis.KeyExistsAsync("game:reward:all");
            var hasSystemStatus = await _redis.KeyExistsAsync("game:system_status");

            return new
            {
                redis = redisStatus.TotalMilliseconds < 100 ? "OK" : "Slow",
                iceLoaded = hasIce,
                rewardLoaded = hasReward,
                systemStatusLoaded = hasSystemStatus
            };
        }
    }
}
