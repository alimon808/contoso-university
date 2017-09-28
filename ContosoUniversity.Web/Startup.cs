using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Web;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContosoUniversity.Services;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Web.Helpers;
using System;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace ContosoUniversity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            CurrentEnvironment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                var relativePathToData = @"../ContosoUniversity.Data";
                var absolutePathToData = System.IO.Path.GetFullPath(relativePathToData);
                var dataProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(absolutePathToData);
                builder.AddJsonFile(dataProvider, $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
                    .AddUserSecrets<Startup>();
            }

            Configuration = builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<ApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ContosoUniversity2017"));
                services.AddDbContext<SecureApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ContosoUniversity2017"));
            }
            else
            {
                if (ContosoUniversity.Services.OperatingSystem.IsMacOs())
                {
                    //store sqlite data base output directory
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var directoryName = System.IO.Path.GetDirectoryName(location);
                    var dataSource = $"Data Source={directoryName}//ContosoDb.sqlite";
                    var secureDataSource = $"Data Source={directoryName}//ContosoDb_secure.sqlite";
                    services.AddDbContext<ApplicationContext>(options => options.UseSqlite(dataSource));
                    services.AddDbContext<SecureApplicationContext>(options => options.UseSqlite(secureDataSource));
                }
                else
                {
                    services.AddDbContext<ApplicationContext>(
                        options => options.UseSqlServer(
                            Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso")));

                    services.AddDbContext<SecureApplicationContext>(
                        options => options.UseSqlServer(
                            Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("IdentityMigration", "Contoso")));
                }
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<SecureApplicationContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IModelBindingHelperAdaptor, DefaultModelBindingHelaperAdaptor>();
            services.AddScoped<IUrlHelperAdaptor, UrlHelperAdaptor>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddMvc();
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });

            services.Configure<IdentityUserOptions>(Configuration.GetSection("IdentityUser"));
            services.Configure<SampleData>(Configuration.GetSection("SampleData"));
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<SMSOptions>(Configuration);
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IDbInitializer dbInitializer)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                dbInitializer.Initialize();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
