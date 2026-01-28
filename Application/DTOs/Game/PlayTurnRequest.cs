namespace FidoDino.Application.DTOs.Game
{
    public class PlayTurnRequest
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }
}