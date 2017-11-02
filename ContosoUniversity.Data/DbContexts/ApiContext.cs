using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data.DbContexts
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string schema = "Contoso";
            if (OperatingSystem.IsMacOs())
            {
                schema = null;
            }

            var config = new DbContextConfig();
            config.ApplicationContextConfig(modelBuilder, schema);
        }
    }
}
