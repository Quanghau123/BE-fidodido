using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Services;
using FidoDino.Domain.Enums.Game;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Leaderboard;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardAppService _leaderboardAppService;
        private readonly ILeaderboardService _leaderboardService;
        public LeaderboardController(ILeaderboardAppService leaderboardAppService, ILeaderboardService leaderboardService)
        {
            _leaderboardAppService = leaderboardAppService;
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// [7.1] Lấy trạng thái user trên BXH
        /// </summary>
        [HttpGet("user-state")]
        public async Task<IActionResult> GetUserLeaderboardState(Guid userId, TimeRangeType timeRange, DateTime? date = null)
        {
            var targetDate = date ?? DateTime.UtcNow;

            var state = await _leaderboardAppService
                .GetUserLeaderboardState(userId, timeRange, targetDate);

            return Ok(state);
        }

        /// <summary>
        /// Lấy top user theo điểm số.
        /// </summary>
        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] string timeRange, [FromQuery] int count)
        {
            var dateStr = Request.Query["date"].ToString();
            DateTime date;
            if (string.IsNullOrWhiteSpace(dateStr))
                date = DateTime.Today;
            else if (!DateTime.TryParse(dateStr, out date))
                return BadRequest("Invalid date (yyyy-MM-dd)");
            if (!Enum.TryParse<TimeRangeType>(timeRange, true, out var timeRangeEnum))
                return BadRequest("Invalid timeRange. Use: Day, Week, Month");
            var result = await _leaderboardService.GetTopAsync(timeRangeEnum, date, count);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thứ hạng của user.
        /// </summary>
        [HttpGet("rank")]
        public async Task<IActionResult> GetUserRank([FromQuery] Guid userId, [FromQuery] string timeRange)
        {
            var dateStr = Request.Query["date"].ToString();
            DateTime date;
            if (string.IsNullOrWhiteSpace(dateStr))
                date = DateTime.Today;
            else if (!DateTime.TryParse(dateStr, out date))
                return BadRequest("Invalid date (yyyy-MM-dd)");
            if (!Enum.TryParse<TimeRangeType>(timeRange, true, out var timeRangeEnum))
                return BadRequest("Invalid timeRange. Use: Day, Week, Month");
            var result = await _leaderboardService.GetUserRankAsync(userId, timeRangeEnum, date);
            return Ok(result);
        }
    }
}