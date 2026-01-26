using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using FidoDino.Common.Exceptions;

namespace FidoDino.Application.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        public LeaderboardService(ILeaderboardRepository leaderboardRepository)
        {
            _leaderboardRepository = leaderboardRepository;
        }
        /// <summary>
        /// Lấy danh sách top người chơi có điểm cao nhất theo khoảng thời gian.
        /// </summary>
        public async Task<IEnumerable<(Guid userId, int score)>> GetTopAsync(string timeRange, int count)
        {
            if (string.IsNullOrWhiteSpace(timeRange))
                throw new ArgumentException("TimeRange is required");
            if (count <= 0)
                throw new ValidationException("Count must be greater than 0");
            return await _leaderboardRepository.GetTopAsync(timeRange, count);
        }
        /// <summary>
        /// Lấy thứ hạng của một người chơi trong bảng xếp hạng theo khoảng thời gian.
        /// </summary>
        public async Task<int> GetUserRankAsync(Guid userId, string timeRange)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(timeRange))
                throw new ArgumentException("TimeRange is required");
            var rank = await _leaderboardRepository.GetUserRankAsync(userId, timeRange);
            if (rank < 0)
                throw new NotFoundException($"User {userId} not found in leaderboard for {timeRange}");
            return rank;
        }
    }
}