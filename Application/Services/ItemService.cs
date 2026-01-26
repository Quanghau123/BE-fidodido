
using FidoDino.Common.Exceptions;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Application.Interface;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IIceRepository _iceRepo;
        private readonly IRewardRepository _rewardRepo;
        private readonly IIceRewardRepository _iceRewardRepo;

        public ItemService(IIceRepository iceRepo, IRewardRepository rewardRepo, IIceRewardRepository iceRewardRepo)
        {
            _iceRepo = iceRepo;
            _rewardRepo = rewardRepo;
            _iceRewardRepo = iceRewardRepo;
        }

        // Ice CRUD
        public async Task<Ice?> GetIceByIdAsync(Guid iceId)
        {
            if (iceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            var ice = await _iceRepo.GetByIdAsync(iceId);
            if (ice == null)
                throw new NotFoundException($"Ice not found with id: {iceId}");
            return ice;
        }
        public async Task<IEnumerable<Ice>> GetAllIceAsync() => await _iceRepo.GetAllAsync();
        // No validation needed for GetAll
        public async Task AddIceAsync(IceRequestAdd ice)
        {
            if (ice == null)
                throw new ArgumentException("Ice is required");
            // Thêm các kiểm tra khác nếu cần
            await _iceRepo.AddAsync(ice);
        }
        // Implementations below are placeholders, update as needed for your logic
        public async Task UpdateIceAsync(IceRequestUpdate ice)
        {
            if (ice == null)
                throw new ArgumentException("Ice is required");
            if (ice.IceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            var existing = await _iceRepo.GetByIdAsync(ice.IceId);
            if (existing == null)
                throw new NotFoundException($"Ice not found with id: {ice.IceId}");
            await _iceRepo.UpdateAsync(ice);
        }
        public async Task DeleteIceAsync(Guid iceId)
        {
            if (iceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            var existing = await _iceRepo.GetByIdAsync(iceId);
            if (existing == null)
                throw new NotFoundException($"Ice not found with id: {iceId}");
            await _iceRepo.DeleteAsync(iceId);
        }

        // Reward CRUD
        public async Task<Reward?> GetRewardByIdAsync(Guid rewardId)
        {
            if (rewardId == Guid.Empty)
                throw new ArgumentException("RewardId is required");
            var reward = await _rewardRepo.GetByIdAsync(rewardId);
            if (reward == null)
                throw new NotFoundException($"Reward not found with id: {rewardId}");
            return reward;
        }
        public async Task<IEnumerable<Reward>> GetAllRewardAsync() => await _rewardRepo.GetAllAsync();
        // No validation needed for GetAll
        public async Task AddRewardAsync(RewardRequestAdd reward)
        {
            if (reward == null)
                throw new ArgumentException("Reward is required");
            await _rewardRepo.AddAsync(reward);
        }
        public async Task UpdateRewardAsync(RewardRequestUpdate reward)
        {
            if (reward == null)
                throw new ArgumentException("Reward is required");
            if (reward.RewardId == Guid.Empty)
                throw new ArgumentException("RewardId is required");
            var existing = await _rewardRepo.GetByIdAsync(reward.RewardId);
            if (existing == null)
                throw new NotFoundException($"Reward not found with id: {reward.RewardId}");
            await _rewardRepo.UpdateAsync(reward);
        }
        public async Task DeleteRewardAsync(Guid rewardId)
        {
            if (rewardId == Guid.Empty)
                throw new ArgumentException("RewardId is required");
            var existing = await _rewardRepo.GetByIdAsync(rewardId);
            if (existing == null)
                throw new NotFoundException($"Reward not found with id: {rewardId}");
            await _rewardRepo.DeleteAsync(rewardId);
        }

        // IceReward CRUD
        public async Task<IEnumerable<IceReward>> GetIceRewardsByIceIdAsync(Guid iceId)
        {
            if (iceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            var iceRewards = await _iceRewardRepo.GetByIceIdAsync(iceId);
            if (iceRewards == null || !iceRewards.Any())
                throw new NotFoundException($"No IceReward found for iceId: {iceId}");
            return iceRewards;
        }
        public async Task AddIceRewardAsync(IceRewardRequestAdd iceReward)
        {
            if (iceReward == null)
                throw new ArgumentException("IceReward is required");
            if (iceReward.IceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            if (iceReward.RewardId == Guid.Empty)
                throw new ArgumentException("RewardId is required");
            // Add more validation as needed
            await _iceRewardRepo.AddAsync(iceReward);
        }
        public async Task UpdateIceRewardAsync(IceRewardRequestUpdate iceReward)
        {
            if (iceReward == null)
                throw new ArgumentException("IceReward is required");
            if (iceReward.IceRewardId == Guid.Empty)
                throw new ArgumentException("IceRewardId is required");
            var existing = await _iceRewardRepo.GetByIdAsync(iceReward.IceRewardId);
            if (existing == null)
                throw new NotFoundException($"IceReward not found with id: {iceReward.IceRewardId}");
            await _iceRewardRepo.UpdateAsync(iceReward);
        }
        public async Task DeleteIceRewardAsync(Guid iceRewardId)
        {
            if (iceRewardId == Guid.Empty)
                throw new ArgumentException("IceRewardId is required");
            var existing = await _iceRewardRepo.GetByIdAsync(iceRewardId);
            if (existing == null)
                throw new NotFoundException($"IceReward not found with id: {iceRewardId}");
            await _iceRewardRepo.DeleteAsync(iceRewardId);
        }
    }
}
