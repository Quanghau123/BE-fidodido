using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IItemService
    {
        // Ice
        Task<Ice?> GetIceByIdAsync(Guid iceId);
        Task<IEnumerable<Ice>> GetAllIceAsync();
        Task AddIceAsync(IceRequestAdd ice);
        Task UpdateIceAsync(IceRequestUpdate ice);
        Task DeleteIceAsync(Guid iceId);

        // Reward
        Task<Reward?> GetRewardByIdAsync(Guid rewardId);
        Task<IEnumerable<Reward>> GetAllRewardAsync();
        Task AddRewardAsync(RewardRequestAdd reward);
        Task UpdateRewardAsync(RewardRequestUpdate reward);
        Task DeleteRewardAsync(Guid rewardId);

        // IceReward
        Task<IEnumerable<IceReward>> GetIceRewardsByIceIdAsync(Guid iceId);
        Task AddIceRewardAsync(IceRewardRequestAdd iceReward);
        Task UpdateIceRewardAsync(IceRewardRequestUpdate iceReward);
        Task DeleteIceRewardAsync(Guid iceRewardId);
    }
}
