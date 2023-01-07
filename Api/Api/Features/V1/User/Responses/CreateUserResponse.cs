namespace Api.Features.V1.User.Responses;

public record CreateUserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;
}