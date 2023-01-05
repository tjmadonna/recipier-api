namespace Api.Features.V1.User.Models;

public record User
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}