using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ContosoUniversity.Data
{
    // modeled from https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("Hosting:Environment")}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            return new ApplicationContext(builder.Options);
        }

        //public ApplicationContext Create()
        //{
        //    var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
        //    var basePath = AppContext.BaseDirectory;

        //    return Create(basePath, environmentName);
        //}

        //private ApplicationContext Create(string basePath, string environmentName)
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(basePath)
        //        .AddJsonFile("appsettings.json")
        //        .AddJsonFile($"appsettings.{environmentName}.json", true)
        //        .AddEnvironmentVariables();

        //    var config = builder.Build();

        //    var connstr = config.GetConnectionString("DefaultConnection");

        //    if (String.IsNullOrWhiteSpace(connstr))
        //    {
        //        throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'");
        //    }

        //    return Create(connstr);
        //}

        //private ApplicationContext Create(string connectionString)
        //{
        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new ArgumentException($"{nameof(connectionString)} is null or empty");
        //    }

        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        //    optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("Migration", "Contoso"));

        //    return new ApplicationContext(optionsBuilder.Options);
        //}

        ////public ApplicationContext Create(DbContextFactoryOptions options)
        ////{
        ////    return Create(options.ContentRootPath, options.EnvironmentName);
        ////}

        //public ApplicationContext Create(DbContextFactoryOptions options)
        //{
        //    return Create(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("Hosting:Environment"));
        //}


    }
}
