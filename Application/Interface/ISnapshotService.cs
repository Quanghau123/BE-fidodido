using System;
using System.Threading.Tasks;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interface
{
    public interface ISnapshotService
    {
        Task SnapshotGameSessionAsync(Guid sessionId);
       Task SnapshotLeaderboardAsync(TimeRangeType timeRange, string timeKey);
    }
}
