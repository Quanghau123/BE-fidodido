using FidoDino.Infrastructure.Data;
using FidoDino.Infrastructure.Data.Seeders;

namespace FidoDino.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task UseDatabaseSeederAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FidoDinoDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(db);
    }
}
