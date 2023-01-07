namespace Api.Settings;

public record JwtSettings
{
    public string AccessSecretKey { get; init; } = string.Empty;

    public string RefreshSecretKey { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public TimeSpan AccessTokenLifetime { get; init; }

    public TimeSpan RefreshTokenLifetime { get; init; }

    public string AccessType = "access";

    public string RefreshType = "refresh";
}