using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.DTOs.Game;
using FidoDino.Application.Interfaces;

namespace FidoDino.Api.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly IPlayTurnService _playTurnService;

        public GameController(IPlayTurnService playTurnService)
        {
            _playTurnService = playTurnService;
        }

        // Random Ice + ShakeCount
        [HttpPost("turn/start")]
        public async Task<ActionResult<StartTurnResultDto>> StartTurn(
            [FromBody] PlayTurnRequest request)
        {
            if (request.UserId == Guid.Empty || request.SessionId == Guid.Empty)
                return BadRequest("Invalid UserId or SessionId");

            var result = await _playTurnService.StartTurnAsync(
                request.UserId,
                request.SessionId);

            return Ok(result);
        }

        // Random Reward + Save DB + Leaderboard
        [HttpPost("turn/end")]
        public async Task<ActionResult<PlayTurnResultDto>> EndTurn([FromBody] EndTurnRequest request)
        {
            if (request.UserId == Guid.Empty)
                return BadRequest("Invalid UserId");

            var result = await _playTurnService.EndTurnAsync(request.UserId);

            return Ok(result);
        }
    }
}
