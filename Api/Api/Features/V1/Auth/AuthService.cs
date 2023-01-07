using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Data;
using Api.Data.Entities;
using Api.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Api.Features.V1.Auth;

public class AuthService : IAuthService
{
    private readonly DataContext _dataContext;

    private readonly JwtSettings _jwtSettings;

    public AuthService(DataContext dataContext, JwtSettings jwtSettings)
    {
        _dataContext = dataContext;
        _jwtSettings = jwtSettings;
    }

    public string CreateAccessToken(Guid userId)
    {
        return CreateToken(
            userId,
            _jwtSettings.AccessType,
            Guid.NewGuid(),
            _jwtSettings.AccessSecretKey,
            _jwtSettings.AccessTokenLifetime
        );
    }
    public async Task<string?> CreateRefreshTokenAsync(Guid userId)
    {
        var jti = Guid.NewGuid();
        await _dataContext.RefreshTokens.AddAsync(new RefreshToken
        {
            Jti = jti,
            ExpiresAt = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenLifetime),
            UserId = userId
        });
        if (await _dataContext.SaveChangesAsync() < 1)
            return null;

        return CreateToken(
            userId,
            _jwtSettings.RefreshType,
            jti,
            _jwtSettings.RefreshSecretKey,
            _jwtSettings.RefreshTokenLifetime
        );
    }

    private string CreateToken(Guid userId, string type, Guid jti, string secret, TimeSpan lifetime)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Audience),
            new Claim(JwtRegisteredClaimNames.Typ, type)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}