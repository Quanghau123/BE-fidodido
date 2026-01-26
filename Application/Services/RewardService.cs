using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common.Exceptions;

namespace FidoDino.Application.Services
{
    public class RewardService : IRewardService
    {
        private readonly IRewardRepository _rewardRepository;
        public RewardService(IRewardRepository rewardRepository)
        {
            _rewardRepository = rewardRepository;
        }
        public async Task<IEnumerable<Reward>> GetRewardsByIceTypeAsync(IceType iceType)
        {
            if (!Enum.IsDefined(typeof(IceType), iceType))
                throw new ArgumentException("Invalid IceType");
            var rewards = await _rewardRepository.GetRewardsByIceTypeAsync(iceType);
            if (rewards == null || !rewards.Any())
                throw new NotFoundException($"No rewards found for ice type: {iceType}");
            return rewards;
        }
    }
}