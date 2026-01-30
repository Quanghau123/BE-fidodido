using FidoDino.Application.Interfaces;
using FidoDino.Common.Exceptions;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Application.Services
{
    public class SnapshotService : ISnapshotService
    {
        private readonly FidoDinoDbContext _db;

        public SnapshotService(FidoDinoDbContext db)
        {
            _db = db;
        }

        // Tạo snapshot (bản lưu) cho một phiên chơi (GameSession) tại thời điểm hiện tại.
        public async Task SnapshotGameSessionAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required");
            var session = await _db.GameSessions.Include(s => s.PlayTurns).FirstOrDefaultAsync(s => s.GameSessionId == sessionId);
            if (session == null)
                throw new NotFoundException($"Game session not found: {sessionId}");
            var now = DateTime.UtcNow;
            var snapshot = new GameSessionSnapshot
            {
                GameSessionSnapshotId = Guid.NewGuid(),
                GameSessionId = session.GameSessionId,
                UserId = session.UserId,
                TotalScore = session.TotalScore,
                LastPlayedAt = session.PlayTurns.OrderByDescending(t => t.PlayedAt).FirstOrDefault()?.PlayedAt,
                CreatedAt = now
            };
            _db.Set<GameSessionSnapshot>().Add(snapshot);
            await _db.SaveChangesAsync();
        }

        // Tạo snapshot (bản lưu) cho bảng xếp hạng (Leaderboard) theo khoảng thời gian và key thời gian.
        public async Task SnapshotLeaderboardAsync(TimeRangeType timeRange, string timeKey)
        {
            if (string.IsNullOrWhiteSpace(timeKey))
                throw new ArgumentException("TimeKey is required");
            var states = await _db.LeaderboardStates
                .Where(s => s.TimeRange == timeRange && s.TimeKey == timeKey)
                .OrderByDescending(s => s.TotalScore)
                .ToListAsync();
            if (states == null || !states.Any())
                throw new NotFoundException($"No leaderboard states found for {timeRange} - {timeKey}");
            int rank = 1;
            var now = DateTime.UtcNow;
            foreach (var state in states)
            {
                var snapshot = new LeaderboardSnapshot
                {
                    LeaderboardSnapshotId = Guid.NewGuid(),
                    TimeRange = timeRange,
                    TimeKey = timeKey,
                    UserId = state.UserId,
                    Rank = rank++,
                    TotalScore = state.TotalScore,
                    CreatedAt = now
                };
                _db.LeaderboardSnapshots.Add(snapshot);
            }
            await _db.SaveChangesAsync();
        }
    }
}
