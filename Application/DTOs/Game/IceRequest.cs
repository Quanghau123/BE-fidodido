using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.DTOs.Game
{
    public class IceRequestAdd
    {
        public IceType IceType { get; set; }
        public int RequiredShake { get; set; }
        public double Probability { get; set; }
    }

    public class IceRequestUpdate
    {
        public Guid IceId { get; set; }
        public IceType IceType { get; set; }
        public int RequiredShake { get; set; }
        public double Probability { get; set; }
    }
}
