
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace CoreWithSwagger.SwaggerVersion
{
    public class ReplaceVersionWithValueInPath : IDocumentFilter
    {
     

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths
                .ToDictionary(
                     p => p.Key.Replace("v{version}", swaggerDoc.Info.Version),
                     p=> p.Value
                );
        }
    }
}
