using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class TimedEffectStateDto
    {
        public EffectType EffectType { get; set; }
        public int RemainingSeconds { get; set; }
        public DateTime? LastActiveAt { get; set; }
    }
}