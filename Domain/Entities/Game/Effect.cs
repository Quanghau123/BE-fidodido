using FidoDino.Domain.Enums;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.Game
{
    public class Effect
    {
        public Guid EffectId { get; set; }
        public EffectType EffectType { get; set; }
        public int DurationSeconds { get; set; }
        public ICollection<ActiveEffect> ActiveEffects { get; set; } = new List<ActiveEffect>();
    }
}