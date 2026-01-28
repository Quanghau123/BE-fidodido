using System;
using System.Threading.Tasks;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface ISnapshotService
    {
        Task SnapshotGameSessionAsync(Guid sessionId);
       Task SnapshotLeaderboardAsync(TimeRangeType timeRange, string timeKey);
    }
}
