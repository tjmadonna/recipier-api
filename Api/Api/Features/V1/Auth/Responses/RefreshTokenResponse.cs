namespace Api.Features.V1.Auth.Responses;

public record RefreshTokenResponse
{
    public string Access { get; init; } = string.Empty;
}