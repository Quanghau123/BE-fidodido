using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums;
using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Interfaces.Leaderboard;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Leaderboard
{
    public class LeaderboardStateRepository : ILeaderboardStateRepository
    {
        private readonly FidoDinoDbContext _context;
        public LeaderboardStateRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Lấy trạng thái bảng xếp hạng của một người chơi theo khoảng thời gian và key thời gian.
        /// </summary>
        public async Task<LeaderboardState?> GetByUserAndTimeAsync(Guid userId, TimeRangeType timeRange, string timeKey)
        {
            return await _context.LeaderboardStates.FirstOrDefaultAsync(x => x.UserId == userId && x.TimeRange == timeRange && x.TimeKey == timeKey);
        }
        /// <summary>
        /// Thêm mới hoặc cập nhật trạng thái bảng xếp hạng của người chơi.
        /// </summary>
        public async Task AddOrUpdateAsync(LeaderboardState state)
        {
            var existing = await GetByUserAndTimeAsync(state.UserId, state.TimeRange, state.TimeKey);
            if (existing == null)
            {
                await _context.LeaderboardStates.AddAsync(state);
            }
            else
            {
                existing.TotalScore = state.TotalScore;
                existing.PlayCount = state.PlayCount;
                existing.AchievedAt = state.AchievedAt;
                existing.StableRandom = state.StableRandom;
                existing.UpdatedAt = state.UpdatedAt;
                _context.LeaderboardStates.Update(existing);
            }
            await _context.SaveChangesAsync();
        }
    }
}