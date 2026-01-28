using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Game
{
    public class PlayTurnRepository : IPlayTurnRepository
    {
        private readonly FidoDinoDbContext _context;
        public PlayTurnRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        public async Task<PlayTurn?> GetByIdAsync(Guid turnId)
        {
            return await _context.PlayTurns.FindAsync(turnId);
        }
        public async Task<IEnumerable<PlayTurn>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.PlayTurns.Where(t => t.GameSessionId == sessionId).ToListAsync();
        }
        public async Task AddAsync(PlayTurn playTurn)
        {
            await _context.PlayTurns.AddAsync(playTurn);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByTimeRangeAsync(TimeRangeType timeRange, string timeKey)
        {
            var states = _context.PlayTurns.Where(x => x.TimeRange == timeRange && x.TimeKey == timeKey);
            _context.PlayTurns.RemoveRange(states);
            await _context.SaveChangesAsync();
        }
    }
}