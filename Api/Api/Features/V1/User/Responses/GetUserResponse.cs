namespace Api.Features.V1.User.Responses;

public record GetUserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;
}