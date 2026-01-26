using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Game
{
    public class IceRewardRepository : IIceRewardRepository
    {
        private readonly FidoDinoDbContext _context;
        public IceRewardRepository(FidoDinoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IceReward>> GetByIceIdAsync(Guid iceId)
        {
            return await _context.IceRewards
                .Where(ir => ir.IceId == iceId)
                .ToListAsync();
        }

        public async Task<IceReward?> GetByIdAsync(Guid iceRewardId)
        {
            return await _context.IceRewards
                .FirstOrDefaultAsync(ir => ir.IceRewardId == iceRewardId);
        }

        public async Task AddAsync(IceRewardRequestAdd iceReward)
        {
            var entity = new IceReward
            {
                IceId = iceReward.IceId,
                RewardId = iceReward.RewardId,
                Probability = iceReward.Probability
            };
            await _context.IceRewards.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IceRewardRequestUpdate iceReward)
        {
            var existing = await _context.IceRewards.FindAsync(iceReward.IceRewardId);
            if (existing == null)
                throw new KeyNotFoundException($"IceReward not found with id: {iceReward.IceRewardId}");
            existing.IceId = iceReward.IceId;
            existing.RewardId = iceReward.RewardId;
            existing.Probability = iceReward.Probability;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid iceRewardId)
        {
            var iceReward = await _context.IceRewards.FindAsync(iceRewardId);
            if (iceReward != null)
            {
                _context.IceRewards.Remove(iceReward);
                await _context.SaveChangesAsync();
            }
        }
    }
}