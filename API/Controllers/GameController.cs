using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Services;
using System;
using System;
using System.Threading.Tasks;
using FidoDino.Application.Interfaces;
using FidoDino.Common;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/game")]
    public class GameController : ControllerBase
    {
        private readonly IGamePlayService _gamePlayService;
        public GameController(IGamePlayService gamePlayService)
        {
            _gamePlayService = gamePlayService;
        }

        /// <summary>
        /// [4.1] Random đá
        /// </summary>
        [HttpGet("random-ice")]
        public async Task<IActionResult> RandomIceAndEffect(Guid userId)
        {
            var result = await _gamePlayService.RandomIceAndApplyEffect(userId);
            return Ok(result);
        }

        /// <summary>
        /// [4.2] Random thưởng
        /// </summary>
        [HttpGet("random-reward")]
        public async Task<IActionResult> RandomReward(Guid iceId)
        {
            var result = await _gamePlayService.RandomReward(iceId);
            return Ok(new ApiResponse<object>(true, "Random success.", result));
        }

        /// <summary>
        /// [4.3] Hoàn thành lượt chơi
        /// </summary>
        [HttpPost("complete-turn")]
        public async Task<IActionResult> CompletePlayTurn(Guid sessionId, Guid iceId, Guid rewardId, int shakeCount, int earnedScore)
        {
            var result = await _gamePlayService.CompletePlayTurn(sessionId, iceId, rewardId, shakeCount, earnedScore);
            return Ok(result);
        }
    }
}