using Microsoft.EntityFrameworkCore;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Infrastructure.Data;
using FidoDino.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FidoDino.Domain.Enums.Game;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.Persistence.Repositories.Game
{
    public class RewardRepository : IRewardRepository
    {
        private readonly FidoDinoDbContext _context;
        public RewardRepository(FidoDinoDbContext context)
        {
            _context = context;
        }
        public async Task<Reward?> GetByIdAsync(Guid rewardId)
        {
            return await _context.Rewards.FindAsync(rewardId);
        }
        public async Task<IEnumerable<Reward>> GetAllAsync()
        {
            return await _context.Rewards.ToListAsync();
        }
        public async Task<IEnumerable<Reward>> GetRewardsByIceTypeAsync(IceType iceType)
        {
            return await _context.Rewards
                .Include(r => r.IceRewards)
                    .ThenInclude(ir => ir.Ice)
                .AsNoTracking()
                .Where(r => r.IceRewards.Any(ir => ir.Ice.IceType == iceType))
                .ToListAsync();
        }
        public async Task AddAsync(RewardRequestAdd reward)
        {
            var entity = new Reward
            {
                RewardName = reward.RewardName,
                Score = reward.Score,
                EffectId = reward.EffectId
            };
            await _context.Rewards.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RewardRequestUpdate reward)
        {
            var existing = await _context.Rewards.FindAsync(reward.RewardId);
            if (existing == null)
                throw new KeyNotFoundException($"Reward not found with id: {reward.RewardId}");
            existing.RewardName = reward.RewardName;
            existing.Score = reward.Score;
            existing.EffectId = reward.EffectId;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid rewardId)
        {
            var reward = await _context.Rewards.FindAsync(rewardId);
            if (reward != null)
            {
                _context.Rewards.Remove(reward);
                await _context.SaveChangesAsync();
            }
        }
    }
}