using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Web;
using ContosoUniversity.Common;
using ContosoUniversity.Web.Helpers;
using ContosoUniversity.Common.Data;
using ContosoUniversity.Common.Interfaces;
using AutoMapper;

namespace ContosoUniversity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            CurrentEnvironment = env;
            Configuration = config;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedContext(Configuration, CurrentEnvironment);
            services.AddCustomizedIdentity(Configuration, CurrentEnvironment);
            services.AddCustomizedAuthentication(Configuration);
            services.AddCustomizedMessage(Configuration);
            services.AddCustomizedMvc(CurrentEnvironment);
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<WebProfile>();
            });

            services.AddScoped<IDbInitializer, WebInitializer>();
            services.AddScoped<IModelBindingHelperAdaptor, DefaultModelBindingHelaperAdaptor>();
            services.AddScoped<IUrlHelperAdaptor, UrlHelperAdaptor>();
            services.AddSingleton<IConfiguration>(Configuration);

            // Call to change httpsport or redirect status code.
            // services.AddHttpsRedirection(options =>
            // {
            //     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //     options.HttpsPort = 20650;
            // });
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                dbInitializer.Initialize();
            }
            else if (env.IsProduction())
            {
                // app.UseExceptionHandler("/Home/Error");
            }

            // aspnetcore 2.1 Require HTTPS
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio
            // enable via config file
            var enableHttps = Configuration["EnableHttps"];
            if (!string.IsNullOrWhiteSpace(enableHttps) && enableHttps.ToLower() == "true")
            {
                // enable https redirection middleware
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
