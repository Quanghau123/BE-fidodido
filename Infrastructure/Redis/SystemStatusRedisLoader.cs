using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using FidoDino.Infrastructure.Data;

namespace FidoDino.Infrastructure.Redis
{
    public class SystemStatusRedisLoader
    {
        private readonly FidoDinoDbContext _db;
        private readonly IDatabase _redis;

        public SystemStatusRedisLoader(FidoDinoDbContext db, IConnectionMultiplexer redis)
        {
            _db = db;
            _redis = redis.GetDatabase();
        }

        /// <summary>
        /// Load toàn trạng thái hệ thống từ database lên Redis
        /// </summary>
        public async Task LoadSystemStatusToRedisAsync()
        {
            var systemStatus = await _db.SystemStatuses.OrderByDescending(x => x.UpdatedAt).FirstOrDefaultAsync();
            if (systemStatus != null)
            {
                await _redis.StringSetAsync("game:system_status", systemStatus.StatusCode.ToString());
            }
        }
    }
}
