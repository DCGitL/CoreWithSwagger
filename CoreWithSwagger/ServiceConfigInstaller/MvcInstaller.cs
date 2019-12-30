using CoreWithSwagger.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace CoreWithSwagger.ServiceConfigInstaller
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //Adding Xmlformatting to the outputFormatter List you can also add your own custom formatter here
            services.AddMvc(config =>
            {
              //  config.Conventions.Add(new ApiExplorerGroupPerVersionConvention());

                config.ReturnHttpNotAcceptable = true;
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                config.Filters.Add(new HttpResponseExceptionFilter()); //Add exception error handler


            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options =>
            {
                //Global validation on posted objects with dataanotations.
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    // TODO: add `using using System.Net.Mime;` to resolve MediaTypeNames
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);


                    return result;

                };
            });


            //Configuration of the web api for versioning
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                //  option.ApiVersionReader = new HeaderApiVersionReader("api-version");
                option.DefaultApiVersion = new ApiVersion(2, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
            });


        }
    }
}
