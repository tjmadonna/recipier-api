namespace Api.Features.V1.Auth;

public interface IAuthService
{
    string CreateAccessToken(Guid userId);

    Task<string?> CreateRefreshTokenAsync(Guid userId);
}