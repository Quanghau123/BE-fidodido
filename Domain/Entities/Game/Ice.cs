using FidoDino.Domain.Enums;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Game
{
    public class Ice
    {
        public Guid IceId { get; set; }
        public IceType IceType { get; set; }
        public int RequiredShake { get; set; }
        public double Probability { get; set; }
        public ICollection<IceReward> IceRewards { get; set; } = new List<IceReward>();
    }
}