using System.ComponentModel.DataAnnotations;

namespace Api.Features.V1.Auth.Requests;

public record SignOutRequest
{
    [Required]
    public string Refresh { get; init; } = string.Empty;
}