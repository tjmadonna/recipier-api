using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Data;
using Api.Data.Entities;
using Api.Features.V1.Core;
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

    public Result<string> CreateAccessToken(Guid userId)
    {
        var token = CreateToken(
            userId,
            _jwtSettings.AccessType,
            Guid.NewGuid(),
            _jwtSettings.AccessSecretKey,
            _jwtSettings.AccessTokenLifetime
        );
        return Result.Success(token);
    }
    public async Task<Result<string>> CreateRefreshTokenAsync(Guid userId)
    {
        var jti = Guid.NewGuid();
        await _dataContext.RefreshTokens.AddAsync(new RefreshToken
        {
            Jti = jti,
            ExpiresAt = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenLifetime),
            UserId = userId
        });
        if (await _dataContext.SaveChangesAsync() < 1)
            return Result.Failure<string>($"Unable to save refresh token with jti {jti}");

        var token = CreateToken(
            userId,
            _jwtSettings.RefreshType,
            jti,
            _jwtSettings.RefreshSecretKey,
            _jwtSettings.RefreshTokenLifetime
        );
        return Result.Success(token);
    }

    public Result<Guid> ValidateRefreshToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
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

            var validatedJwtToken = (JwtSecurityToken)validatedToken;
            var jti = validatedJwtToken.Claims.GetJti();
            if (jti == null)
                return Result.Failure<Guid>("Unable to get jti claim from list of claims");

            var tokenExists = _dataContext.RefreshTokens.Any(r => r.Jti == jti);
            if (!tokenExists)
                return Result.Failure<Guid>("Unable to find token in list of approved tokens");

            var userId = validatedJwtToken.Claims.GetSub();
            if (userId == null)
                return Result.Failure<Guid>("Unable to get sub claim from list of claims");

            return Result.Success<Guid>((Guid)userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Failure<Guid>(e.Message); ;
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