using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FidoDino.Persistence.Repositories.Game
{
    public class EffectRepository : IEffectRepository
    {
        private readonly FidoDinoDbContext _context;
        public EffectRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        public async Task<Effect?> GetByIdAsync(Guid effectId)
        {
            return await _context.Effects.FindAsync(effectId);
        }
        public async Task<IEnumerable<Effect>> GetAllAsync()
        {
            return await _context.Effects.ToListAsync();
        }
    }
}