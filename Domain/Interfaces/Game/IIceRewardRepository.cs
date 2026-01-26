using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IIceRewardRepository
    {
        Task<IEnumerable<IceReward>> GetByIceIdAsync(Guid iceId);
        Task<IceReward?> GetByIdAsync(Guid iceRewardId);
        Task AddAsync(IceRewardRequestAdd iceReward);
        Task UpdateAsync(IceRewardRequestUpdate iceReward);
        Task DeleteAsync(Guid iceRewardId);
    }
}