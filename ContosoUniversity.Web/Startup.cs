using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Web;
using ContosoUniversity.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContosoUniversity
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
                services.AddDbContext<ApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase());
            }
            else
            {
                services.AddDbContext<ApplicationContext>(
                    options => options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso")));

                services.AddDbContext<SecureApplicationContext>(
                    options => options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("IdentityMigration", "Contoso")));
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<SecureApplicationContext>()
                    .AddDefaultTokenProviders();
            }

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IModelBindingHelperAdaptor, DefaultModelBindingHelaperAdaptor>();
            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                DbInitializer.Initialize(context);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
