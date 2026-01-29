using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Game;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/game-sessions")]
    public class GameSessionController : ControllerBase
    {
        private readonly IGameSessionService _gameSessionService;
        public GameSessionController(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        /// <summary>
        /// Lấy phiên chơi đang hoạt động của người dùng.
        /// Dùng để khôi phục phiên khi người chơi reload hoặc quay lại game.
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<GameSession?>> GetActiveSession([FromQuery] Guid userId)
        {
            var session = await _gameSessionService.GetActiveSessionAsync(userId);

            if (session == null)
                return NoContent();

            return Ok(session);
        }

        /// <summary>
        /// Bắt đầu session
        /// </summary>
        [HttpPost("start")]
        public async Task<ActionResult<GameSessionDto>> StartSession([FromQuery] Guid userId)
        {
            var session = await _gameSessionService.StartSessionAsync(userId);
            return Ok(session);
        }

        /// <summary>
        /// Kết thúc session
        /// </summary>
        [HttpPost("end")]
        public async Task<IActionResult> EndSession([FromQuery] Guid sessionId)
        {
            await _gameSessionService.EndSessionAsync(sessionId);
            return NoContent();
        }
    }
}
