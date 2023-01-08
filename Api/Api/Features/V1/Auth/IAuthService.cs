using Api.Features.V1.Core;

namespace Api.Features.V1.Auth;

public interface IAuthService
{
    Result<string> CreateAccessToken(Guid userId);

    Task<Result<Guid>> AuthenticateAsync(string email, string password);

    Task<Result<string>> CreateRefreshTokenAsync(Guid userId);

    Task<Result> DeleteRefreshTokenAsync(string token);

    Task<Result> DeleteAllRefreshTokensAsync(string token);

    Result<Guid> ValidateRefreshToken(string token);
}