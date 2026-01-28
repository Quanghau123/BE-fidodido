using StackExchange.Redis;
using System.Text.Json;
using FidoDino.Domain.Enums.Game;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.Infrastructure.Redis
{
    public class IceCacheService
    {
        private readonly IDatabase _redis;
        public IceCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<List<IceResultDto>> GetAllIcesAsync()
        {
            var entries = await _redis.HashGetAllAsync("game:ice:all");
            var list = new List<IceResultDto>();
            foreach (var entry in entries)
            {
                try
                {
                    var iceObj = JsonSerializer.Deserialize<IceRedisModel>(entry.Value!);
                    if (iceObj != null)
                    {
                        list.Add(new IceResultDto
                        {
                            IceId = iceObj.IceId,
                            IceType = iceObj.IceType,
                            ShakeCount = iceObj.RequiredShake,
                            Probability = iceObj.Probability
                        });
                    }
                }
                catch { }
            }
            return list;
        }

        private class IceRedisModel
        {
            public Guid IceId { get; set; }
            public IceType IceType { get; set; }
            public int RequiredShake { get; set; }
            public double Probability { get; set; }
        }
    }
}
