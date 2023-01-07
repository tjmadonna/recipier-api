using System.ComponentModel.DataAnnotations;

namespace Api.Features.V1.User.Requests;

public record CreateUserRequest
{
    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}