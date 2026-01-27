// using FidoDino.Application.Interfaces;
// using FidoDino.Domain.Enums.Game;
// using Microsoft.AspNetCore.Mvc;

// namespace FidoDino.API.Controllers
// {
//     [ApiController]
//     [Route("api/v1/leaderboard/summary")]
//     public class LeaderboardSummaryController : ControllerBase
//     {
//         private readonly ILeaderboardSummaryService _leaderboardSummaryService;
//         public LeaderboardSummaryController(ILeaderboardSummaryService leaderboardSummaryService)
//         {
//             _leaderboardSummaryService = leaderboardSummaryService;
//         }

//         /// <summary>
//         /// [8.1] Tổng kết và reset BXH
//         /// </summary>
//         [HttpPost("summarize-and-reset")]
//         public async Task<IActionResult> SummarizeAndReset([FromQuery] string timeRange)
//         {
//             var dateStr = Request.Query["date"].ToString();
//             DateTime date;
//             if (string.IsNullOrWhiteSpace(dateStr))
//                 date = DateTime.Today;
//             else if (!DateTime.TryParse(dateStr, out date))
//                 return BadRequest("Invalid date (yyyy-MM-dd)");
//             if (!Enum.TryParse<TimeRangeType>(timeRange, true, out var timeRangeEnum))
//                 return BadRequest("Invalid timeRange. Use: Day, Week, Month");

//             await _leaderboardSummaryService.SummarizeAndResetAsync(timeRangeEnum, date, topN: 100);

//             return Ok();
//         }
//     }
// }