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

            var config = new DbContextConfig();
            config.SecureApplicationContextConfig(builder, dbSchema);
        }
    }
}
