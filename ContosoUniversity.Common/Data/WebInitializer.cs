using ContosoUniversity.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ContosoUniversity.Data.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ContosoUniversity.Data;

namespace ContosoUniversity.Common.Data
{
    public class WebInitializer : IDbInitializer
    {
        private readonly ApplicationContext _context;
        private readonly SecureApplicationContext _secureContext;
        private readonly ILogger _logger;
        private readonly SampleData _data;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityUserOptions _identityUser;
        private readonly IHostingEnvironment _environment;

        public WebInitializer(ApplicationContext context, 
            SecureApplicationContext secureContext, 
            ILoggerFactory loggerFactory,
            IOptions<SampleData> dataOptions,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityUserOptions> identityUserOptions, IHostingEnvironment env)
        {
            _context = context;
            _secureContext = secureContext;
            _logger = loggerFactory.CreateLogger("DbInitializer");
            _data = dataOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _identityUser = identityUserOptions.Value;
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
            if (_context.Database.EnsureCreated())
            {
                _logger.LogInformation("Creating database schema...");

                // create identity tables
                if (!_environment.IsEnvironment("Testing"))
                {
                    _secureContext.Database.Migrate();
                }
            }
            var unitOfWork = new UnitOfWork(_context);
            var seedData = new SeedData(_logger, unitOfWork, _data);
            seedData.Initialize();
        }

        private async Task InitializeSecureContext()
        {
            // abort if Administrator role exists
            if (_secureContext.Roles.Any(r => r.Name == _identityUser.Role))
            {
                return;
            }

            // create Administrator role
            await _roleManager.CreateAsync(new IdentityRole("Administrator"));

            string user = _identityUser.UserName;
            string password = _identityUser.Password;
            await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, EmailConfirmed = true }, password);
            await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
        }
    }
}
