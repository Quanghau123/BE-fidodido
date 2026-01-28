using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class IceResultDto
    {
        public Guid IceId { get; set; }
        public IceType IceType { get; set; }
        public int ShakeCount { get; set; }
        public double Probability { get; set; }
    }
}
