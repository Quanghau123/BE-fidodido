using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Auth;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FidoDino.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user,
            //IEnumerable để linh hoạt nhận mọi kiểu danh sách (List, mảng, kết quả LINQ) mà không bị phụ thuộc cứng.
            IEnumerable<string> permissions)
        {
            var handler = new JwtSecurityTokenHandler();
            //HMAC-SHA256 chỉ làm việc với nhị phân
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            foreach (var permission in permissions)
                claims.Add(new Claim("permission", permission));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpireMinutes"]!)
                ),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            return handler.WriteToken(
                handler.CreateToken(tokenDescriptor));
        }

        // Giải mã và xác thực JWT token, trả về ClaimsPrincipal chứa thông tin user
        public ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = true)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!);

            return handler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = validateLifetime,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                },
                out _);
        }
    }
}
