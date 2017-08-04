using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Identity
{
    public class SecureApplicationContext : IdentityDbContext<ApplicationUser>
    {
        private const string _dbSchema = "Contoso";

        public SecureApplicationContext(DbContextOptions<SecureApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users", _dbSchema);
            builder.Entity<IdentityRole>().ToTable("Role", _dbSchema);
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", _dbSchema);
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole", _dbSchema);
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", _dbSchema);
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", _dbSchema);
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken", _dbSchema);
        }
    }
}
