using CoreWithSwagger.SwaggerFilters;
using CoreWithSwagger.SwaggerFilters.Version;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace CoreWithSwagger.ServiceConfigInstaller
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            //Register the Swagger generator, defining 1 or more swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new Info
                {
                    Title = "v1.0 Core API ",
                    Description = "Swagger Core Api",
                    Version = "v1.0"
                });
                c.SwaggerDoc("v2.0", new Info
                {
                    Title = "v2.0 Core API ",
                    Description = "Swagger Core Api",
                    Version = "v2.0"
                });

                // Apply the filters
                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithValueInPath>();
                c.OperationFilter<AuthoricationHeaderOperationFilter>();

                c.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out MethodInfo methodInfo))
                        return false;
                    var versions = methodInfo.DeclaringType.GetCustomAttributes(true)
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == version);
                });



            });

        }
    }
}
