using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class AuthenticateContext : IdentityDbContext
{
    public AuthenticateContext(DbContextOptions<AuthenticateContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>(e => { e.ToTable("Users"); });
        builder.Entity<IdentityRole>(e => { e.ToTable("Roles"); });
        builder.Entity<IdentityUserRole<string>>(e => { e.ToTable("UsersRoles"); });
        builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable("UsersClaims"); });
        builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable("UsersLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable("RolesClaims"); });
        builder.Entity<IdentityUserToken<string>>(e => { e.ToTable("UsersTokens"); });
    }
}