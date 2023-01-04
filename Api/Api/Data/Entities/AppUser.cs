using Microsoft.AspNetCore.Identity;

namespace Api.Data.Entities;

public class AppUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}