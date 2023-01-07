using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Data;
using Api.Data.Entities;
using Api.Features.V1.Core.Extensions;
using Api.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Api.Features.V1.Auth;

public class AuthService : IAuthService
{
    private readonly DataContext _dataContext;

    private readonly TokenValidationParameters _refreshTokenValidationParameters;

    private readonly JwtSettings _jwtSettings;

    private readonly ILogger<AuthService> _logger;

    public AuthService(
        DataContext dataContext,
        TokenValidationParameters refreshTokenValidationParameters,
        JwtSettings jwtSettings,
        ILogger<AuthService> logger
    )
    {
        _dataContext = dataContext;
        _refreshTokenValidationParameters = refreshTokenValidationParameters;
        _jwtSettings = jwtSettings;
        _logger = logger;
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

    public Guid? ValidateRefreshToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.RefreshSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidTypes = new[] { _jwtSettings.RefreshType },
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userId = claimsPrincipal.GetSub();
            if (userId == null)
                return null;

            var tokenExists = _dataContext.RefreshTokens.Any(r => r.Jti == userId);
            if (!tokenExists)
                return null;

            return userId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
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