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

        public async Task<List<(LeaderboardSnapshot Snapshot, string UserName)>> GetTopAsync(TimeRangeType timeRange, string timeKey, int topN)
        {
            var query = from snap in _db.LeaderboardSnapshots.AsNoTracking()
                        join user in _db.Users.AsNoTracking() on snap.UserId equals user.UserId
                        where snap.TimeRange == timeRange && snap.TimeKey == timeKey
                        orderby snap.Rank
                        select new { Snapshot = snap, user.UserName };
            var result = await query.Take(topN).ToListAsync();
            return result.Select(x => (x.Snapshot, x.UserName)).ToList();
        }
    }
}
