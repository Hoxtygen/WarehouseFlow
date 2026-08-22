using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Infrastructure.Implementations;

public sealed class TokenService(IOptions<JwtSettings> options) : ITokenService
{
    public string GenerateToken(
        string userId,
        string email,
        string userName,
        string role
    )
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Secret))
        {
            throw new InvalidOperationException("JWT secret is not configured.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(userName) ? email : userName),
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}