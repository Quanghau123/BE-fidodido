using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interfaces;
using FidoDino.Application.Interface;

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


        /// <summary>
        /// [5.1] Lấy hiệu ứng đang active
        /// </summary>
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
                var duration = await db.StringGetAsync(key);
                effects.Add(new { EffectType = effectType, Duration = duration.ToString() });
            }
            return Ok(effects);
        }

        /// <summary>
        /// [5.2] Kiểm tra user có hiệu ứng
        /// </summary>
        [HttpGet("has-effect")]
        public async Task<ActionResult<bool>> HasEffect([FromQuery] Guid userId, [FromQuery] string effectType)
        {
            var has = await _effectService.HasEffectAsync(userId, effectType);
            return Ok(has);
        }

        /// <summary>
        /// [5.3] Set hiệu ứng
        /// </summary>
        [HttpPost("set-effect")]
        public async Task<IActionResult> SetEffect([FromQuery] Guid userId, [FromQuery] string effectType, [FromQuery] int durationSeconds)
        {
            await _effectService.SetEffectAsync(userId, effectType, durationSeconds);
            return NoContent();
        }

        /// <summary>
        /// [5.4] Xóa hiệu ứng
        /// </summary>
        [HttpDelete("remove-effect")]
        public async Task<IActionResult> RemoveEffect([FromQuery] Guid userId, [FromQuery] string effectType)
        {
            await _effectService.RemoveEffectAsync(userId, effectType);
            return NoContent();
        }
    }
}
