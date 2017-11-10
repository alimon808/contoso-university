using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using Microsoft.Extensions.Configuration;

namespace ContosoUniversity.Tests
{
    public abstract class BaseIntegrationTest<T> where T : class
    {
        private const string SolutionName = "ContosoUniversity.sln";
        public string AntiForgeryFormTokenName { get; set; }
        public string AntiForgeryCookieName { get; set; }

        public BaseIntegrationTest()
        {
            AntiForgeryFormTokenName = "AntiForgeryFormTokenName";
            AntiForgeryCookieName = "AntiForgeryCookieName";
        }

        protected TestServer InitTestServer()
        {
            var startupAssembly = typeof(T).GetTypeInfo().Assembly;
            string solutionRelativeTargetProjectParentDir = string.Empty;
            var contentRoot = GetProjectPath(solutionRelativeTargetProjectParentDir, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Testing")
                .ConfigureAppConfiguration(ConfigConfiguration)
                .UseStartup(typeof(T));

            var ts = new TestServer(builder);
            ts.BaseAddress = new Uri("https://localhost/");

            return ts;
        }

        protected void ConfigConfiguration(WebHostBuilderContext context, IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true)
                .AddJsonFile("testData.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<T>()
                .AddEnvironmentVariables();
        }

        protected void InitializeServices(IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = AntiForgeryCookieName;
                options.FormFieldName = AntiForgeryFormTokenName;
            });

            var startupAssembly = typeof(T).GetTypeInfo().Assembly;

            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));

            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }

        /// <summary>
        /// Gets the full path to the target project path that we wish to test
        /// </summary>
        /// <param name="solutionRelativePath">
        /// The parent directory of the target project.
        /// e.g. src, samples, test, or test/Websites
        /// </param>
        /// <param name="startupAssembly">The target project's assembly.</param>
        /// <returns>The full path to the target project.</returns>
        private static string GetProjectPath(string solutionRelativePath, Assembly startupAssembly)
        {
            // Get name of the target project which we want to test
            var projectName = startupAssembly.GetName().Name;

            // Get currently executing test project path
            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            // Find the folder which contains the solution file. We then use this information to find the target
            // project which we want to test.
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, SolutionName));
                if (solutionFileInfo.Exists)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath, projectName));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }
}
