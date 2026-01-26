using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace FidoDino.Application.Services
{
    public class LeaderboardAppService : ILeaderboardAppService
    {
        private readonly ILeaderboardStateRepository _stateRepo;
        public LeaderboardAppService(ILeaderboardStateRepository stateRepo)
        {
            _stateRepo = stateRepo;
        }
        /// <summary>
        /// Lấy trạng thái bảng xếp hạng của người dùng theo khoảng thời gian và key thời gian.
        /// </summary>
        public async Task<LeaderboardState?> GetUserLeaderboardState(Guid userId, TimeRangeType timeRange, string timeKey)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(timeKey))
                throw new ArgumentException("TimeKey is required");
            var state = await _stateRepo.GetByUserAndTimeAsync(userId, timeRange, timeKey);
            if (state == null)
                throw new NotFoundException($"Leaderboard state not found for userId: {userId}, timeRange: {timeRange}, timeKey: {timeKey}");
            return state;
        }
        /// <summary>
        /// Thêm mới hoặc cập nhật trạng thái bảng xếp hạng của người dùng.
        /// </summary>
        public async Task AddOrUpdateLeaderboardState(LeaderboardState state)
        {
            if (state == null)
                throw new ValidationException("Leaderboard state is required.");
            if (state.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(state.TimeKey))
                throw new ArgumentException("TimeKey is required");
            await _stateRepo.AddOrUpdateAsync(state);
        }
    }
}