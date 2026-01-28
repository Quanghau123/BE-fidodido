namespace FidoDino.Application.DTOs.Game
{
    public class TurnCacheDto
    {
        public Guid SessionId { get; set; }
        public Guid IceId { get; set; }
        public int ShakeCount { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
