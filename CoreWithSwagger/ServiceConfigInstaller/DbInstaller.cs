using CoreWithSwagger.Helper;
using CoreWithSwagger.Models;
using CoreWithSwagger.Models.IdentityDbContext;
using CoreWithSwagger.SerivceExtensions;
using CoreWithSwagger.Services;
using EmployeeDBDal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreWithSwagger.ServiceConfigInstaller
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("EmployDbStore")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            var  connectionString = configuration.GetConnectionString("dbadventureworks");
            services.AddTransient<IRepository>(s => new Repository(connectionString));

            //this is a extension method from EmployeeDBDal
            services.ServicesEmpDb(configuration);

            // configure strongly typed settings objects for the jwt authentication from any existing db
            var appSettingsSection = configuration.GetSection("AppSettings");
            var tokensecret = appSettingsSection.GetSection("Secret").Value;
            var tokenlifeTime = appSettingsSection.GetSection("TokenLifeTime").Value;
            var employeeStoreConnectionstring = configuration.GetConnectionString("EmployDbStore");

            services.Configure<AppSettings>(options =>
            {
                options.Secret = tokensecret;
                options.TokenLifeTime = TimeSpan.Parse(tokenlifeTime);
                options.DatabaseConnectionstring = employeeStoreConnectionstring;
            });




            //configure the jwt authentication
            services.AddJwtService(configuration);



            // configure DI for application services that handle Jwt authentication
            services.AddScoped<IUserService, UserService>();

        }
    }
}
