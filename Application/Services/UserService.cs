using FidoDino.Application.DTOs.User;
using FidoDino.Application.Interfaces;
using FidoDino.Infrastructure.Data;
using FidoDino.Common.Exceptions;
using FidoDino.Domain.Enums.User;

using Microsoft.EntityFrameworkCore;

using System.Text;
using FidoDino.Domain.Entities.Auth;

namespace FidoDino.Application.Services
{
    public class UserService : IUserService
    {
        private readonly FidoDinoDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(FidoDinoDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<object> CreateNewUserAsync(CreateUserRequest data)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == data.Email);

            if (exists)
                throw new ConflictException("User with this email already exists.");

            var user = new User
            {
                UserName = data.UserName.Trim(),
                Email = data.Email.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password),
                IsActive = true,
                UserRole = string.IsNullOrWhiteSpace(data.UserRole)
                    ? FidoDino.Domain.Enums.User.UserRole.Player
                    : Enum.Parse<FidoDino.Domain.Enums.User.UserRole>(data.UserRole, true)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new { UserId = user.UserId };
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.UserRole.ToString(),
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<object> HandleUpdateUserAsync(Guid userId, UpdateUserRequest data)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            if (data.UserName != null)
                user.UserName = data.UserName.Trim();

            if (!string.IsNullOrWhiteSpace(data.Email) && data.Email.Trim() != user.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == data.Email.Trim() && u.UserId != userId);

                if (emailExists)
                    throw new ConflictException("Another user with this email already exists.");

                user.Email = data.Email.Trim();
            }

            if (data.UserRole.HasValue)
                user.UserRole = data.UserRole.Value;

            if (data.IsActive.HasValue)
                user.IsActive = data.IsActive.Value;

            await _context.SaveChangesAsync();

            return new { Message = "User updated successfully." };
        }

        public async Task<object> HandleDeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new { Message = "User deleted successfully." };
        }

        public async Task<Stream> ExportUsersToCsvAsync(ExportUserRequest request, CancellationToken ct)
        {
            try
            {
                var query = _context.Users.AsNoTracking();

                if (request.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == request.IsActive.Value);

                if (!string.IsNullOrWhiteSpace(request.UserRole) &&
                    Enum.TryParse<UserRole>(request.UserRole, true, out var role))
                    query = query.Where(x => x.UserRole == role);

                var users = await query
                    .OrderBy(x => x.UserName)
                    .ToListAsync(ct);

                var stream = new MemoryStream();
                using var writer = new StreamWriter(
                    stream,
                    new UTF8Encoding(true),
                    leaveOpen: true);

                writer.WriteLine("Id;UserName;Email;IsActive;UserRole");

                foreach (var user in users)
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(500, ct); // để test
                    writer.WriteLine($"{user.UserId};{user.UserName};{user.Email};{user.IsActive};{user.UserRole}");
                }

                //FlushAsync() → đẩy dữ liệu vào stream
                await writer.FlushAsync();
                //quay về đầu stream nếu không → file download trống
                stream.Position = 0;
                return stream;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Export users canceled by client");
                throw;
            }
        }
    }
}
