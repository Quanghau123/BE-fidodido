using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Common.Exceptions;
using FidoDino.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly FidoDinoDbContext? _db;

        public LeaderboardService(
            ILeaderboardRepository leaderboardRepository,
            FidoDinoDbContext? db = null)
        {
            _leaderboardRepository = leaderboardRepository;
            _db = db;
        }

        // Update điểm người chơi và sync lên Redis leaderboard
        public async Task UpdateUserScoreAsync(
            LeaderboardState state,
            TimeRangeType timeRange,
            DateTime date)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (!Enum.IsDefined(typeof(TimeRangeType), timeRange))
                throw new ArgumentException("TimeRange is required");

            if (date == default)
                throw new ArgumentException("Date is required");

            var compositeScore = LeaderboardScoreCalculator.Calculate(state);
            var realScore = state.TotalScore;
            await _leaderboardRepository.UpdateScoreAsync(
                timeRange,
                date,
                state.UserId,
                compositeScore,
                realScore);
        }

        // Lấy top leaderboard theo thời gian
        public async Task<IEnumerable<LeaderboardUserDto>> GetTopAsync(
            TimeRangeType timeRange,
            DateTime date,
            int count)
        {
            if (!Enum.IsDefined(typeof(TimeRangeType), timeRange))
                throw new ArgumentException("TimeRange is required");

            if (date == default)
                throw new ArgumentException("Date is required");

            if (count <= 0)
                throw new ValidationException("Count must be greater than 0");

            var topList = await _leaderboardRepository
                .GetTopAsync(timeRange, date, count);

            var userIds = topList.Select(x => x.userId).ToList();

            var userDict = new Dictionary<Guid, string>();
            if (_db != null)
            {
                userDict = _db.Users
                    .Where(u => userIds.Contains(u.UserId))
                    .ToDictionary(u => u.UserId, u => u.UserName);
            }

            return topList.Select(x => new LeaderboardUserDto
            {
                UserId = x.userId,
                UserName = userDict.TryGetValue(x.userId, out var name)
                    ? name
                    : x.userId.ToString(),
                Score = x.realScore,
                CompositeScore = x.compositeScore
            });
        }

        // Lấy thứ hạng của user trong leaderboard
        public async Task<int> GetUserRankAsync(
            Guid userId,
            TimeRangeType timeRange,
            DateTime date)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (!Enum.IsDefined(typeof(TimeRangeType), timeRange))
                throw new ArgumentException("TimeRange is required");

            if (date == default)
                throw new ArgumentException("Date is required");

            var rank = await _leaderboardRepository
                .GetUserRankAsync(timeRange, date, userId);

            if (!rank.HasValue)
                throw new NotFoundException(
                    $"User {userId} not found in leaderboard {timeRange} at {date:yyyy-MM-dd}");

            return rank.Value;
        }
    }
}
