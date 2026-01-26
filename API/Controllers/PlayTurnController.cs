using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Game;
using FidoDino.Application.Interface;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/play-turn")]
    public class PlayTurnController : ControllerBase
    {
        private readonly IPlayTurnService _playTurnService;
        public PlayTurnController(IPlayTurnService playTurnService)
        {
            _playTurnService = playTurnService;
        }

        /// <summary>
        /// [4.4] Xử lý lượt chơi tổng hợp
        /// </summary>
        [HttpPost("play")]
        public async Task<ActionResult<PlayTurnResultDto>> PlayTurn([FromQuery] Guid userId, [FromQuery] Guid sessionId)
        {
            var result = await _playTurnService.PlayTurnAsync(userId, sessionId);
            return Ok(result);
        }
    }
}
