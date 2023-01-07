using Api.Features.V1.Core;

namespace Api.Features.V1.Auth;

public interface IAuthService
{
    Result<string> CreateAccessToken(Guid userId);

    Task<Result<string>> CreateRefreshTokenAsync(Guid userId);

    Result<Guid> ValidateRefreshToken(string token);
}