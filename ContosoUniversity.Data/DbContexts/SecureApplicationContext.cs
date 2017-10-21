using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;

namespace ContosoUniversity.Data.DbContexts
{
    public class SecureApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public SecureApplicationContext(DbContextOptions<SecureApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            string dbSchema = "Contoso";
            if (OperatingSystem.IsMacOs())
            {
                dbSchema = null;
            }

            builder.Entity<ApplicationUser>().ToTable("Users", dbSchema);
            builder.Entity<IdentityRole>().ToTable("Role", dbSchema);
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", dbSchema);
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole", dbSchema);
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", dbSchema);
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", dbSchema);
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken", dbSchema);
        }
    }
}
