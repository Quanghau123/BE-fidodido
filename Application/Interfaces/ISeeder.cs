using FidoDino.Infrastructure.Data;

namespace FidoDino.Application.Interfaces
{
    public interface ISeeder
    {
        Task SeedAsync(FidoDinoDbContext context);
    }
}
