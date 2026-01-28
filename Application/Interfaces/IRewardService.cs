using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IRewardService
    {
        Task<IEnumerable<Reward>> GetRewardsByIceTypeAsync(IceType iceType);
    }
}