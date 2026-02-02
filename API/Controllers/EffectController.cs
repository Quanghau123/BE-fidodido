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
            var effects = new List<object>();

            // Lấy timed effects (không bao gồm BlockPlay)
            var timedEffects = await _effectService.GetActiveTimedEffectsAsync(userId);
            foreach (var effect in timedEffects)
            {
                effects.Add(new { EffectType = effect.EffectType.ToString(), Duration = effect.RemainingSeconds });
            }

            // Lấy BlockPlay nếu còn hiệu lực
            if (await _effectService.HasEffectAsync(userId, EffectType.BlockPlay))
            {
                var duration = await _effectService.GetEffectDurationAsync(userId, EffectType.BlockPlay);
                effects.Add(new { EffectType = "BlockPlay", Duration = duration });
            }

            // Lấy utility effect (AutoBreakIce)
            var utilityCount = await _effectService.GetUtilityRemainAsync(userId);
            if (utilityCount > 0)
            {
                effects.Add(new { EffectType = "AutoBreakIce", Duration = utilityCount });
            }

            return Ok(effects);
        }
    }
}
