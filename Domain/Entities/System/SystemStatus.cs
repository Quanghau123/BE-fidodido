using FidoDino.Domain.Enums;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Entities.System
{
    public class SystemStatus
    {
        public Guid SystemStatusId { get; set; }
        public SystemStatusType StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}