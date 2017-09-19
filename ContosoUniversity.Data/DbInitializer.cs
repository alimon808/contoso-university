using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context, ILoggerFactory loggerFactory)
        {
            context.Database.EnsureCreated();

            var unitOfWork = new UnitOfWork(context);
            var seedData = new SeedData(loggerFactory.CreateLogger("SeedData"), unitOfWork);
            seedData.Initialize();
        }
    }
}
