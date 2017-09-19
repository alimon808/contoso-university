using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            CurrentEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<ApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ContosoUniversity2017"));
            } else
            {
                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso"));
                });
            }
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ApplicationContext context, ILoggerFactory loggerFactory)
        {
            if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsEnvironment("Testing"))
            {
                DbInitializer.Initialize(context, loggerFactory);
            }

            app.UseMvc();
            //app.UseMvcWithDefaultRoute();
        }
    }
}
