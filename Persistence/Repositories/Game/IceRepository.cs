using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FidoDino.Persistence.Repositories.Game
{
    public class IceRepository : IIceRepository
    {
        private readonly FidoDinoDbContext _context;
        public IceRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        public async Task<Ice?> GetByIdAsync(Guid iceId)
        {
            return await _context.Ices
        .Include(i => i.IceRewards)
            .ThenInclude(ir => ir.Reward)
                .ThenInclude(r => r.Effect)
        .FirstOrDefaultAsync(i => i.IceId == iceId);
        }
        public async Task<IEnumerable<Ice>> GetAllAsync()
        {
            return await _context.Ices.ToListAsync();
        }

        public async Task AddAsync(IceRequestAdd ice)
        {
            var entity = new Ice
            {
                IceType = ice.IceType,
                RequiredShake = ice.RequiredShake,
                Probability = ice.Probability
            };
            await _context.Ices.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(IceRequestUpdate ice)
        {
            var entity = await _context.Ices.FindAsync(ice.IceId);
            if (entity == null)
                throw new KeyNotFoundException($"Ice not found with id: {ice.IceId}");
            entity.IceType = ice.IceType;
            entity.RequiredShake = ice.RequiredShake;
            entity.Probability = ice.Probability;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid iceId)
        {
            var ice = await _context.Ices.FindAsync(iceId);
            if (ice != null)
            {
                _context.Ices.Remove(ice);
                await _context.SaveChangesAsync();
            }
        }
    }
}