using ContosoUniversity.Data;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using ContosoUniversity.Common.Data;
using AutoMapper;
using ContosoUniversity.Common.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ContosoUniversity.Common
{
    // cleaner startup: 
    // http://odetocode.com/blogs/scott/archive/2016/08/30/keeping-a-clean-startup-cs-in-asp-net-core.aspx
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedContext(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment env)
        {
            if (env.IsEnvironment("Testing"))
            {
                services.AddDbContext<ApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
                services.AddDbContext<SecureApplicationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
                services.AddDbContext<WebContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
                services.AddDbContext<ApiContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                if (OperatingSystem.IsMacOs())
                {
                    //store sqlite data base output directory
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var directoryName = System.IO.Path.GetDirectoryName(location);
                    var dataSource = $"Data Source={directoryName}//ContosoDb.sqlite";
                    var secureDataSource = $"Data Source={directoryName}//ContosoDb_secure.sqlite";
                    services.AddDbContext<ApplicationContext>(options => options.UseSqlite(dataSource));
                    services.AddDbContext<WebContext>(options => options.UseSqlite(dataSource));
                    services.AddDbContext<ApiContext>(options => options.UseSqlite(dataSource));
                    services.AddDbContext<SecureApplicationContext>(options => options.UseSqlite(dataSource));
                }
                else
                {
                    services.AddDbContext<ApplicationContext>(
                        options => options.UseSqlServer(
                            configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("Migration", "Contoso")));

                    services.AddDbContext<SecureApplicationContext>(
                        options => options.UseSqlServer(
                            configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("IdentityMigration", "Contoso")));
                    services.AddDbContext<WebContext>(
                        options => options.UseSqlServer(
                            configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("IdentityMigration", "Contoso")));
                    services.AddDbContext<ApiContext>(
                        options => options.UseSqlServer(
                            configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsHistoryTable("IdentityMigration", "Contoso")));
                }
            }

            services.AddScoped<UnitOfWork<ApplicationContext>, UnitOfWork<ApplicationContext>>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<,>));

            services.Configure<SampleData>(configuration.GetSection("SampleData"));

            return services;
        }


        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IHostingEnvironment env)
        {
            services.AddMvc();

            if (env.IsProduction())
            {
                services.AddMvc().AddMvcOptions(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }

            return services;
        }

        // identity 2.0
        // oauth - facebook, google
        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
                // issue generating sms friendly verification code
                // https://github.com/aspnet/Identity/issues/1388
                config.Tokens.ChangePhoneNumberTokenProvider = "Phone";
            })
            .AddEntityFrameworkStores<SecureApplicationContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });

            services.Configure<IdentityUserOptions>(configuration.GetSection("IdentityUser"));

            return services;
        }

        public static IServiceCollection AddCustomizedAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    options.AppId = configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = configuration["Authentication:Tokens:Issuer"],
                        ValidAudience = configuration["Authentication:Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Tokens:Key"]))
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomizedApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = configuration["Authentication:Tokens:Issuer"],
                        ValidAudience = configuration["Authentication:Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Tokens:Key"]))
                    };
                });

            return services;
        }

        // sms and email services
        public static IServiceCollection AddCustomizedMessage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AuthMessageSenderOptions>(configuration);
            services.Configure<SMSOptions>(configuration);

            return services;
        }


        public static IServiceCollection AddCustomizedAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<DepartmentDTO, Department>().ReverseMap();
            });

            return services;
        }
    }
}
