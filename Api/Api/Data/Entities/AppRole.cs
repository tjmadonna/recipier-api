using Microsoft.AspNetCore.Identity;

namespace Api.Data.Entities;

public class AppRole : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppRole() : base()
    {
    }

    public AppRole(string roleName) : base(roleName)
    {
    }
}