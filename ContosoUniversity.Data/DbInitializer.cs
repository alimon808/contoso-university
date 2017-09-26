using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Data
{
    // public interface IDbInitializer
    // {
    //     void Initialize();
    // }

    // public class SecureDbInitializer : IDbInitializer
    // {
    //     private readonly SecureApplicationContext _context;
    //     private readonly ILogger _logger;
    //     public SecureDbInitializer(SecureApplicationContext context, ILogger logger)
    //     {
    //         _context = context;
    //         _logger = logger;
    //     }
    //     public void Initialize()
    //     {
    //         // create database schema if it does not exist
    //         if (_context.Database.EnsureCreated())
    //         {
    //             _logger.LogInformation("Creating database schema...");
    //         }
    //     }
    // }

    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context, SecureApplicationContext secureApplicationContext, ILoggerFactory loggerFactory, SampleData data)
        {
            context.Database.EnsureCreated();
            if (secureApplicationContext != null)
            {
                secureApplicationContext.Database.EnsureCreated();
            }

            var unitOfWork = new UnitOfWork(context);
            var seedData = new SeedData(loggerFactory.CreateLogger("SeedData"), unitOfWork, data);
            seedData.Initialize();
        }
    }
}
