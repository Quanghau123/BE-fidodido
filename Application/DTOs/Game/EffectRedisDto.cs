using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class EffectRedisDto
    {
        public Guid EffectId { get; set; }
        public EffectType EffectType { get; set; }
        public int DurationSeconds { get; set; }
    }
}