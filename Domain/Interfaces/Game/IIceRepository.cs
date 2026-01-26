using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IIceRepository
    {
        Task<Ice?> GetByIdAsync(Guid iceId);
        Task<IEnumerable<Ice>> GetAllAsync();
        Task AddAsync(IceRequestAdd ice);
        Task UpdateAsync(IceRequestUpdate ice);
        Task DeleteAsync(Guid iceId);
    }
}