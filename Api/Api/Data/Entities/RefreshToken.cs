using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Entities;

[Index(nameof(ExpiresAt))]
public class RefreshToken
{
    [Key]
    public Guid Jti { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = default!;
}