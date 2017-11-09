using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigConfiguration)
                .ConfigureLogging(ConfigureLogger)
                .UseStartup<Startup>()
                .Build();

        // refs: 
        // https://wildermuth.com/2017/07/06/Program-cs-in-ASP-NET-Core-2-0
        public static void ConfigConfiguration(WebHostBuilderContext context, IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory());

            if (context.HostingEnvironment.IsDevelopment()) 
            {
                config.AddJsonFile($"sampleData.json", optional: true, reloadOnChange: false);
                config.AddUserSecrets<Startup>();
            }
            
            config.AddEnvironmentVariables();
        }

        static void ConfigureLogger(WebHostBuilderContext ctx, ILoggingBuilder logging)
        {
            logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.AddDebug();
        }
    }
}
