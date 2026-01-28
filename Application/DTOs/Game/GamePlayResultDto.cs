using FidoDino.Domain.Entities.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class GamePlayResultDto
    {
        public IceResultDto Ice { get; set; } = default!;
        public RewardResultDto Reward { get; set; } = default!;
        public int ShakeCount { get; set; }
        public int EarnedScore { get; set; }
    }
}