using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Services;
using FidoDino.Domain.Enums.Game;
using FidoDino.Application.Interfaces;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardAppService _leaderboardService;
        public LeaderboardController(ILeaderboardAppService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }
        /// <summary>
        /// [7.1] Lấy trạng thái user trên BXH
        /// </summary>
        [HttpGet("user-state")]
        public async Task<IActionResult> GetUserLeaderboardState(Guid userId, TimeRangeType timeRange, string timeKey)
        {
            var state = await _leaderboardService.GetUserLeaderboardState(userId, timeRange, timeKey);
            return Ok(state);
        }
    }
}