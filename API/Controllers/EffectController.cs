using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/effects")]
    public class EffectController : ControllerBase
    {
        private readonly IEffectService _effectService;
        public EffectController(IEffectService effectService)
        {
            _effectService = effectService;
        }

        // Lấy hiệu ứng đang active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEffects([FromQuery] Guid userId, [FromServices] StackExchange.Redis.IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints()[0]);
            var pattern = $"game:effect:{userId}:*";
            var keys = server.Keys(pattern: pattern).ToArray();
            var effects = new List<object>();
            foreach (var key in keys)
            {
                var effectType = key.ToString().Split(':').Last();
                var ttl = await db.KeyTimeToLiveAsync(key);
                effects.Add(new { EffectType = effectType, Duration = ttl?.TotalSeconds ?? 0 });
            }

            var utilityVal = await db.StringGetAsync($"effect:utility:{userId}");
            if (utilityVal.HasValue && int.TryParse(utilityVal, out var utilityCount) && utilityCount > 0)
            {
                effects.Add(new { EffectType = "AutoBreakIce", Duration = utilityCount });
            }

            return Ok(effects);
        }
    }
}
