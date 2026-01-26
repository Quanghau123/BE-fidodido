using FidoDino.Domain.Interfaces.Game;
using FidoDino.Domain.Entities.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Game
{
    public class ActiveEffectRepository : IActiveEffectRepository
    {
        private readonly FidoDinoDbContext _context;
        public ActiveEffectRepository(FidoDinoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActiveEffect>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.ActiveEffects
                .Where(ae => ae.GameSessionId == sessionId)
                .ToListAsync<ActiveEffect>();
        }
        public async Task AddAsync(ActiveEffect activeEffect)
        {
            await _context.ActiveEffects.AddAsync(activeEffect);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid activeEffectId)
        {
            var activeEffect = await _context.ActiveEffects
                .FirstOrDefaultAsync(ae => ae.ActiveEffectId == activeEffectId);

            if (activeEffect != null)
            {
                _context.ActiveEffects.Remove(activeEffect);
                await _context.SaveChangesAsync();
            }
        }
    }
}
