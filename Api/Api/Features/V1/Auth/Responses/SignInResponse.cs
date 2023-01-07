namespace Api.Features.V1.Auth.Responses;

public record SignInResponse
{
    public string Access { get; init; } = string.Empty;

    public string Refresh { get; init; } = string.Empty;
}