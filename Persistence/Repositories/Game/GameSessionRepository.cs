using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Game
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly FidoDinoDbContext _context;
        public GameSessionRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        public async Task<GameSession?> GetByIdAsync(Guid sessionId)
        {
            return await _context.GameSessions.FindAsync(sessionId);
        }
        public async Task<IEnumerable<GameSession>> GetActiveSessionsAsync()
        {
            return await _context.GameSessions.Where(s => s.EndTime == null).ToListAsync();
        }
        public async Task AddAsync(GameSession session)
        {
            await _context.GameSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(GameSession session)
        {
            _context.GameSessions.Update(session);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<GameSession>> GetByUserIdAsync(Guid userId)
        {
            return await _context.GameSessions.Where(s => s.UserId == userId).ToListAsync();
        }
        public async Task<GameSession?> GetActiveSessionByUserIdAsync(Guid userId)
        {
            return await _context.GameSessions.FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
        }
        public async Task DeleteByTimeRangeAsync(TimeRangeType timeRange, string timeKey)
        {
            var states = _context.GameSessions.Where(x => x.TimeRange == timeRange && x.TimeKey == timeKey);
            _context.GameSessions.RemoveRange(states);
            await _context.SaveChangesAsync();
        }
    }
}