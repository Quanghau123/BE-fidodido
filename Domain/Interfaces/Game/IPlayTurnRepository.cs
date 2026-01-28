using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IPlayTurnRepository
    {
        Task<PlayTurn?> GetByIdAsync(Guid turnId);
        Task<IEnumerable<PlayTurn>> GetBySessionIdAsync(Guid sessionId);
        Task AddAsync(PlayTurn playTurn);
        Task DeleteByTimeRangeAsync(TimeRangeType timeRange, string timeKey);
    }
}