using Microsoft.AspNetCore.Mvc;
using FidoDino.Domain.Enums.Game;
using FidoDino.Application.Interfaces;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardAppService _leaderboardAppService;
        private readonly ILeaderboardService _leaderboardService;
        private readonly TimeRangeType _defaultTimeRange;
        public LeaderboardController(ILeaderboardAppService leaderboardAppService, ILeaderboardService leaderboardService, IConfiguration config)
        {
            _leaderboardAppService = leaderboardAppService;
            _leaderboardService = leaderboardService;

            Enum.TryParse(config["Leaderboard:DefaultTimeRange"], true, out _defaultTimeRange);
            if (_defaultTimeRange == 0)
                _defaultTimeRange = TimeRangeType.Day;
        }

        /// <summary>
        /// Lấy thứ hạng của user.
        /// </summary>
        [HttpGet("rank")]
        public async Task<IActionResult> GetUserRank([FromQuery] Guid userId)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var result = await _leaderboardService.GetUserRankAsync(userId, _defaultTimeRange, now);
            return Ok(result);
        }
    }
}