using CoreWithSwagger.CustomRouteConstraints;
using CoreWithSwagger.ExceptionHandling;
using CoreWithSwagger.MiddleWare;
using CoreWithSwagger.ServiceConfigInstaller;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreWithSwagger
{
    public class Startup
    {
       

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //get the configuration object for the apikey
            services.Configure<ApiKey>(Configuration.GetSection("ApiKey"));

            //Register/Add the RouteConstraints for lat long
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("LatLongConstraint", typeof(DoubleRouteLatLongConstraint));
            });

            //Adding Cross Origin Resource Sharing (CORS) 
            services.AddCors( options => 
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true));
            } );

            services.AddInstaller(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
               // app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
                //app.UseHsts();
            }

            //Register the custom middleware 
            app.UseMiddleware<HeaderMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            //Enable Authentication globally for this api
            app.UseAuthentication();

            //Enable middleware to serve swagger-ui (HTML, JS,CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
              
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
                c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "v2.0");

            });

            // global cors policy make sure it is applied before Usemvc
            app.UseCors("CorsPolicy");

         

            app.UseMvc();


        }
    }
}
