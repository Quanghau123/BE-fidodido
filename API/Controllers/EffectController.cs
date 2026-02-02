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
        public async Task<IActionResult> GetActiveEffects([FromQuery] Guid userId)
        {
            var effects = await _effectService.GetAllActiveEffectsAsync(userId);
            return Ok(effects);
        }
    }
}
