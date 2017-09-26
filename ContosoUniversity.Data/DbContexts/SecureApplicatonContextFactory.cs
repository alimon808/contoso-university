using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using ContosoUniversity.Data;

namespace ContosoUniversity.Data.DbContexts
{
    public class SecureApplicationContextFactory : IDesignTimeDbContextFactory<SecureApplicationContext>
    {
        public SecureApplicationContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("Hosting:Environment")}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<SecureApplicationContext>();

            if (ContosoUniversity.Services.OperatingSystem.IsMacOs())
            {
                builder.UseSqlite("Data Source=ContosoUniversity.sqlite");
            }
            else
            {
                builder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            }
            return new SecureApplicationContext(builder.Options);
        }
    }
}
