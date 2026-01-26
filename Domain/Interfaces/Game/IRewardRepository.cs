using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IRewardRepository
    {
        Task<Reward?> GetByIdAsync(Guid rewardId);
        Task<IEnumerable<Reward>> GetAllAsync();
        Task<IEnumerable<Reward>> GetRewardsByIceTypeAsync(IceType iceType);
        Task AddAsync(RewardRequestAdd reward);
        Task UpdateAsync(RewardRequestUpdate reward);
        Task DeleteAsync(Guid rewardId);
    }
}