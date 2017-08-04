using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;

namespace ContosoUniversity.Data
{
    // modeled from https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory
    public class ApplicationContextFactory : IDbContextFactory<ApplicationContext>
    {
        public ApplicationContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
            var basePath = AppContext.BaseDirectory;
            return Create(basePath, environmentName);
        }

        private ApplicationContext Create(string basePath, string environmentName)
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

        private ApplicationContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} is null or empty");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("Migration", "Contoso"));

            return new ApplicationContext(optionsBuilder.Options);
        }

        public ApplicationContext Create(DbContextFactoryOptions options)
        {
            return Create(options.ContentRootPath, options.EnvironmentName);
        }
    }
}
