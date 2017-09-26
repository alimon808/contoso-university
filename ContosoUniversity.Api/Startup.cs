using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Data.Interfaces;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Services;

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
            }
            else
            {
                if (OperatingSystem.IsMacOs())
                {
                    //store sqlite data base output directory
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var directoryName = System.IO.Path.GetDirectoryName(location);
                    var dataSource = $"Data Source={directoryName}//ContosoDb.sqlite";
                    services.AddDbContext<ApplicationContext>(options => options.UseSqlite(dataSource));
                }
                else
                {

                    services.AddDbContext<ApplicationContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso"));
                    });
                }
            }

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ApplicationContext context, ILoggerFactory loggerFactory)
        {
            if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsEnvironment("Testing"))
            {
                var sampleData = new SampleData();
                Configuration.GetSection("SampleData").Bind(sampleData);
                //todo: redesign
                DbInitializer.Initialize(context, null, loggerFactory, sampleData);
            }

            app.UseMvc();
            //app.UseMvcWithDefaultRoute();
        }
    }
}
