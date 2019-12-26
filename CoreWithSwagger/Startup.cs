using CoreWithSwagger.ExceptionHandling;
using CoreWithSwagger.Helper;
using CoreWithSwagger.MiddleWare;
using CoreWithSwagger.Models;
using CoreWithSwagger.SerivceExtensions;
using CoreWithSwagger.Services;
using EmployeeDBDal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Text;

namespace CoreWithSwagger
{
    public class Startup
    {
        private string connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = configuration.GetConnectionString("dbadventureworks");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adding Cross Origin Resource Sharing (CORS) 
            services.AddCors();

            services.AddTransient<IRepository>(s => new Repository(connectionString));

            //this is a extension method from EmployeeDBDal
            services.ServicesEmpDb(Configuration);

            // configure strongly typed settings objects for the jwt authentication from any existing db
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var tokensecret = appSettingsSection.GetSection("Secret").Value;
            var employeeStoreConnectionstring = Configuration.GetConnectionString("EmployDbStore");

            services.Configure<AppSettings>(options =>
            {
                options.Secret = tokensecret;
                options.DatabaseConnectionstring = employeeStoreConnectionstring;
            });

            //Configuration of the web api for versioning
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.ApiVersionReader = new HeaderApiVersionReader("api-version");
                option.DefaultApiVersion = new ApiVersion(2, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            //Adding Xmlformatting to the outputFormatter List you can also add your own custom formatter here
            services.AddMvc(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                config.Filters.Add(new HttpResponseExceptionFilter()); //Add exception error handler


            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    // TODO: add `using using System.Net.Mime;` to resolve MediaTypeNames
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);


                    return result;

                };
            });

            //Register the Swagger generator, defining 1 or more swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Core API",
                    Description = "Swagger Core Api",
                    Version = "v1"
                });
              
            });


            //configure the jwt authentication
            services.AddJwtService(Configuration);

           

            // configure DI for application services that handle Jwt authentication
            services.AddScoped<IUserService, UserService>();

          
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                //app.UseHsts();
            }

            //Register the custom middleware 
            app.UseMiddleware<HeaderMiddleware>();

            //Enable Authentication globally for this api
            app.UseAuthentication();

            //Enable middleware to serve swagger-ui (HTML, JS,CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core Api");

            });

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

         

            app.UseMvc();


        }
    }
}
