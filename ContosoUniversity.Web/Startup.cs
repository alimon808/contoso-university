using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Web;
using ContosoUniversity.Common;
using ContosoUniversity.Web.Helpers;
using Microsoft.AspNetCore.Rewrite;
using ContosoUniversity.Common.Data;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment() || env.IsEnvironment("Testing"))
            {
                //var relativePathToData = @"../ContosoUniversity.Data";
                //var absolutePathToData = System.IO.Path.GetFullPath(relativePathToData);
                //var dataProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(absolutePathToData);
                builder.AddJsonFile($"sampleData.json", optional: true, reloadOnChange: false);
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder
                .AddJsonFile("appsettings.production.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedContext(Configuration, CurrentEnvironment);
            services.AddCustomizedIdentity(Configuration);
            services.AddCustomizedMessage(Configuration);
            services.AddCustomizedMvc();
            services.AddScoped<IDbInitializer, WebInitializer>();
            services.AddScoped<IModelBindingHelperAdaptor, DefaultModelBindingHelaperAdaptor>();
            services.AddScoped<IUrlHelperAdaptor, UrlHelperAdaptor>();
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
            
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
