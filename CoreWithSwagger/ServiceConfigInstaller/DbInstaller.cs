using CoreWithSwagger.Helper;
using CoreWithSwagger.Models;
using CoreWithSwagger.SerivceExtensions;
using CoreWithSwagger.Services;
using EmployeeDBDal.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreWithSwagger.ServiceConfigInstaller
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
           var  connectionString = configuration.GetConnectionString("dbadventureworks");
            services.AddTransient<IRepository>(s => new Repository(connectionString));

            //this is a extension method from EmployeeDBDal
            services.ServicesEmpDb(configuration);

            // configure strongly typed settings objects for the jwt authentication from any existing db
            var appSettingsSection = configuration.GetSection("AppSettings");
            var tokensecret = appSettingsSection.GetSection("Secret").Value;
            var employeeStoreConnectionstring = configuration.GetConnectionString("EmployDbStore");

            services.Configure<AppSettings>(options =>
            {
                options.Secret = tokensecret;
                options.DatabaseConnectionstring = employeeStoreConnectionstring;
            });




            //configure the jwt authentication
            services.AddJwtService(configuration);



            // configure DI for application services that handle Jwt authentication
            services.AddScoped<IUserService, UserService>();

        }
    }
}
