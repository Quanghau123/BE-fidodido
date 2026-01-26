using FidoDino.Domain.Entities.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class IceRewardRequestAdd
    {
        public Guid IceId { get; set; }
        public Guid RewardId { get; set; }
        public double Probability { get; set; }
    }
    public class IceRewardRequestUpdate
    {
        public Guid IceRewardId { get; set; }
        public Guid IceId { get; set; }
        public Guid RewardId { get; set; }
        public double Probability { get; set; }
    }
}
