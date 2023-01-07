using System.ComponentModel.DataAnnotations;

namespace Api.Features.V1.Auth.Requests;

public record RefreshTokenRequest
{
    [Required]
    public string Refresh { get; init; } = string.Empty;
}