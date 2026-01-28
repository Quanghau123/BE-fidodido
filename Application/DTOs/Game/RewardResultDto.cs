using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class RewardResultDto
    {
        public Guid RewardId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public int Score { get; set; }
        public EffectType EffectType { get; set; }
    }
}
