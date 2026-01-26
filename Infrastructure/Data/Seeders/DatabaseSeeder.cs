using FidoDino.Application.Interfaces;

namespace FidoDino.Infrastructure.Data.Seeders
{
    public class DatabaseSeeder
    {
        private readonly IEnumerable<ISeeder> _seeders;

        public DatabaseSeeder(IEnumerable<ISeeder> seeders)
        {
            _seeders = seeders;
        }

        public async Task SeedAsync(FidoDinoDbContext context)
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync(context);
            }
        }
    }
}
