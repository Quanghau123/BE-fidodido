using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}