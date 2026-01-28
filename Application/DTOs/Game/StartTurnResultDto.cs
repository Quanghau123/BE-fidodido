namespace FidoDino.Application.DTOs.Game
{
    public class StartTurnResultDto
    {
        public Guid IceId { get; set; }
        public string IceType { get; set; } = default!;
        public int ShakeCount { get; set; }
    }
}
