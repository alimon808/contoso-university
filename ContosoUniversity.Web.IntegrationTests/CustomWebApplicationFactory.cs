using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common.Data;
using ContosoUniversity.Data.Entities;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Data.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.TestHost;

namespace ContosoUniversity.Web.IntegrationTests
{
    public class TestDbInitializer : IDbInitializer
    {
        public void Initialize()
        {
            // todo
        }
    }
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider
                var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                // Add a database context (WebContext) using an in-memory database for testing
                services.AddDbContext<WebContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Add a database context (SecureApplicationContext) using an in-memory database for testing
                services.AddDbContext<SecureApplicationContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemorySecureDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                // Add a database context (ApplicationContext) using an in-memory database for testing
                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                    db.Database.EnsureCreated();
                    Utilities.InitializeDbForTest(db);
                }
            });

            builder.UseEnvironment("Testing")
                   .ConfigureTestServices(services =>
                    {
                        services.AddScoped<IDbInitializer, TestDbInitializer>();
                    });

        }
    }
}
