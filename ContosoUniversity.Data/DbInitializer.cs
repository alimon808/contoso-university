using ContosoUniversity.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ContosoUniversity.Data.Interfaces;
using Microsoft.Extensions.Options;

namespace ContosoUniversity.Data
{

    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationContext _context;
        private readonly SecureApplicationContext _secureContext;
        private readonly ILogger _logger;
        private readonly SampleData _data;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationContext context, 
            SecureApplicationContext secureContext, 
            ILoggerFactory loggerFactory,
            IOptions<SampleData> dataOptions,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _secureContext = secureContext;
            _logger = loggerFactory.CreateLogger("DbInitializer");
            _data = dataOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
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
            }
            var unitOfWork = new UnitOfWork(_context);
            var seedData = new SeedData(_logger, unitOfWork, _data);
            seedData.Initialize();
        }

        private async Task InitializeSecureContext()
        {
            // create schema if database does not exist
            // todo: schema will not be created
            // workaround - use dotnet ef database update on the command line
            //if (_secureContext.Database.EnsureCreated())
            //{
            //    _logger.LogInformation("Creating identity database schema...");
            //}

            // abort if Administrator role exists
            if (_secureContext.Roles.Any(r => r.Name == "Administrator"))
            {
                return;
            }

            // create Administrator role
            await _roleManager.CreateAsync(new IdentityRole("Administrator"));

            // create the default admin account and apply the administrator role
            // todo: get user/pass from user secrets
            string user = "abc@example.com";
            string password = "P@ssw0rd!";
            await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, EmailConfirmed = true }, password);
            await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
        }
    }
}
