namespace Api.Features.V1.User.Models;

public record UserInfo
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;
}