using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;

namespace ContosoUniversity.Identity
{
    // modeled from https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory
    public class SecureApplicationContextFactory : IDbContextFactory<SecureApplicationContext>
    {
        public SecureApplicationContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
            var basePath = AppContext.BaseDirectory;
            return Create(basePath, environmentName);
        }

        private SecureApplicationContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("settings.json")
                .AddJsonFile($"settings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("DefaultConnection");

            if (String.IsNullOrWhiteSpace(connstr))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'");
            }

            return Create(connstr);
        }

        private SecureApplicationContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} is null or empty");
            }

            var optionsBuilder = new DbContextOptionsBuilder<SecureApplicationContext>();
            optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("IdentityMigration", "Contoso"));

            return new SecureApplicationContext(optionsBuilder.Options);
        }

        public SecureApplicationContext Create(DbContextFactoryOptions options)
        {
            return Create(options.ContentRootPath, options.EnvironmentName);
        }
    }
}
