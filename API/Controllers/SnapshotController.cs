using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interface;
using System;
using System.Threading.Tasks;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/snapshot")]
    public class SnapshotController : ControllerBase
    {
        private readonly ISnapshotService _snapshotService;
        public SnapshotController(ISnapshotService snapshotService)
        {
            _snapshotService = snapshotService;
        }

        /// <summary>
        /// [8.1] Snapshot GameSession
        /// </summary>
        [HttpPost("gamesession")]
        public async Task<IActionResult> SnapshotGameSession([FromQuery] Guid sessionId)
        {
            await _snapshotService.SnapshotGameSessionAsync(sessionId);
            return Ok(new { success = true, message = "Snapshot completed." });
        }

        /// <summary>
        /// [8.2] Snapshot Leaderboard
        /// </summary>
        [HttpPost("leaderboard")]
        public async Task<IActionResult> SnapshotLeaderboard([FromQuery] TimeRangeType timeRange, [FromQuery] string timeKey)
        {
            await _snapshotService.SnapshotLeaderboardAsync(timeRange, timeKey);
            return Ok(new { success = true, message = "Snapshot completed." });
        }
    }
}
