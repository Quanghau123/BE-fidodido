using FidoDino.Domain.Entities;
using FidoDino.Domain.Entities.Auth;
using System.Security.Claims;

namespace FidoDino.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user, IEnumerable<string> permissions);
        ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = true);
    }
}