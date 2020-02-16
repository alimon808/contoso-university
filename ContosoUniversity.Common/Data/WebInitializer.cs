using ContosoUniversity.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ContosoUniversity.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ContosoUniversity.Data;

namespace ContosoUniversity.Common.Data
{
    public class WebInitializer : IDbInitializer
    {
        private readonly WebContext _webContext;
        private readonly ILogger _logger;
        private readonly SampleData _data;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AdminIdentityOptions _adminIdentityUser;
        private readonly IHostingEnvironment _environment;

        public WebInitializer(WebContext webContext,
            ILoggerFactory loggerFactory,
            IOptions<SampleData> dataOptions,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AdminIdentityOptions> adminIdentityOptions, IHostingEnvironment env)
        {
            _webContext = webContext;
            _logger = loggerFactory.CreateLogger("DbInitializer");
            _data = dataOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminIdentityUser = adminIdentityOptions.Value;
            _environment = env;
        }
        public void Initialize()
        {
            InitializeContext();
            InitializeSecureContext().Wait();
        }

        private void InitializeContext()
        {
            // create database schema if it does not exist
            if (_webContext.Database.EnsureCreated())
            {
                _logger.LogInformation("Creating database schema...");
            }

            var unitOfWork = new UnitOfWork<WebContext>(_webContext);
            var seedData = new SeedData<WebContext>(_logger, unitOfWork, _data);
            seedData.Initialize();
        }

        private async Task InitializeSecureContext()
        {
            // abort if Administrator role exists
            if (_webContext.Roles.Any(r => r.Name == _adminIdentityUser.Role))
            {
                return;
            }

            // create Administrator role
            await _roleManager.CreateAsync(new IdentityRole(_adminIdentityUser.Role));

            string user = _adminIdentityUser.UserName;
            string password = _adminIdentityUser.Password;
            await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, EmailConfirmed = true }, password);
            await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
        }
    }
}
