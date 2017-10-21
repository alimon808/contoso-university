using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Data.Interfaces;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Services;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;
using Microsoft.AspNetCore.Identity;

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
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsEnvironment("Testing"))
            {
                if (OperatingSystem.IsMacOs())
                {
                    //store sqlite data base output directory
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var directoryName = System.IO.Path.GetDirectoryName(location);
                    var dataSource = $"Data Source={directoryName}//ContosoDb.sqlite";
                    services.AddDbContext<ApplicationContext>(options => options.UseSqlite(dataSource));
                    services.AddDbContext<SecureApplicationContext>(options => options.UseSqlite(dataSource));
                }
                else
                {
                    services.AddDbContext<ApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
                    services.AddDbContext<SecureApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
                }
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
                    services.AddDbContext<SecureApplicationContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso"));
                    });
                }
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<SecureApplicationContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddMvc();

            services.Configure<SampleData>(Configuration.GetSection("SampleData"));
            services.Configure<IdentityUserOptions>(Configuration.GetSection("IdentityUser"));
        }

        public void Configure(IApplicationBuilder app, ApplicationContext context, ILoggerFactory loggerFactory, IDbInitializer dbInitializer)
        {
            if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsEnvironment("Testing"))
            {
                var sampleData = new SampleData();
                Configuration.GetSection("SampleData").Bind(sampleData);
                //todo: redesign
                dbInitializer.Initialize();
            }

            app.UseMvc();
            //app.UseMvcWithDefaultRoute();
        }
    }
}
