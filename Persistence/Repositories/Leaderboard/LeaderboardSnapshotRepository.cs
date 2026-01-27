using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;
using FidoDino.Domain.Interfaces.Leaderboard;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Leaderboard
{
    public class LeaderboardSnapshotRepository : ILeaderboardSnapshotRepository
    {
        private readonly Infrastructure.Data.FidoDinoDbContext _db;
        public LeaderboardSnapshotRepository(Infrastructure.Data.FidoDinoDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LeaderboardSnapshot>> GetByTimeRangeAsync(TimeRangeType timeRange, string timeKey)
        {
            return await _db.LeaderboardSnapshots
                .AsNoTracking()
                .Where(x => x.TimeRange == timeRange && x.TimeKey == timeKey)
                .ToListAsync();
        }

        public async Task AddAsync(LeaderboardSnapshot snapshot)
        {
            await _db.LeaderboardSnapshots.AddAsync(snapshot);
            await _db.SaveChangesAsync();
        }
    }
}
