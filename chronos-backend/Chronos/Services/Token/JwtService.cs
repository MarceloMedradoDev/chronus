using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chronos.Models;
using Microsoft.IdentityModel.Tokens;
using Chronos.Interfaces;

namespace Chronos.Services.Token
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? configuration["jwt:secretKey"];

            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                      ?? configuration["jwt:issuer"];

            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                        ?? configuration["jwt:audience"];
        }

        public string GenerateToken(UserModel user)
        {
            var claims = new[]
            {
            new Claim("id", user.Id.ToString()),
            new Claim("Registration", user.Registration.ToString()),
            new Claim(ClaimTypes.Role, user.Type.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}