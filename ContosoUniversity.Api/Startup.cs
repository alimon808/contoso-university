using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Rewrite;
using ContosoUniversity.Common;
using ContosoUniversity.Common.Data;
using ContosoUniversity.Common.Interfaces;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment() || env.IsEnvironment("Testing"))
            {
                builder.AddJsonFile($"sampleData.json", optional: true, reloadOnChange: false);
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedContext(Configuration, CurrentEnvironment);
            services.AddCustomizedAutoMapper();
            services.AddCustomizedMvc();

            services.AddScoped<IDbInitializer, ApiInitializer>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IDbInitializer dbInitializer)
        {
            if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsEnvironment("Testing"))
            {
                dbInitializer.Initialize();
            }
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            app.UseMvc();
        }
    }
}
