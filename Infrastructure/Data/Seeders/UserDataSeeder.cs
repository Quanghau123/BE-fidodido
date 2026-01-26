using FidoDino.Domain.Entities;
using FidoDino.Application.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using FidoDino.Domain.Entities.Auth;

namespace FidoDino.Infrastructure.Data.Seeders
{
    public class UserDataSeeder : ISeeder
    {
        public async Task SeedAsync(FidoDinoDbContext context)
        {
            string jsonPath = "Infrastructure/Data/Seeders/users.json";
            await SeedFromJsonAsync(context, jsonPath);
        }

        private async Task SeedFromJsonAsync(FidoDinoDbContext context, string jsonPath)
        {
            if (!File.Exists(jsonPath)) return;

            var json = await File.ReadAllTextAsync(jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    var existingUser = await context.Users
                        .FirstOrDefaultAsync(u => u.UserId == user.UserId);

                    if (existingUser == null)
                    {
                        context.Users.Add(user);
                    }
                    else
                    {
                        context.Entry(existingUser).CurrentValues.SetValues(user);
                    }
                }

                var userIdsInJson = users.Select(u => u.UserId).ToHashSet();
                var usersToDelete = context.Users.Where(u => !userIdsInJson.Contains(u.UserId));
                context.Users.RemoveRange(usersToDelete);

                await context.SaveChangesAsync();
            }
        }
    }
}
