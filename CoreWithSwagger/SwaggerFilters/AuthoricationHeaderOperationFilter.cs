using CoreWithSwagger.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace CoreWithSwagger.SwaggerFilters
{
    public class AuthoricationHeaderOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            bool isAuthorized = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter );
            bool allowAnonymous = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if(isAuthorized && !allowAnonymous)
            {
                if(operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();

                }

                operation.Parameters.Add(
                    new NonBodyParameter
                    {
                        Description = "JWT Authorization header using the bearer sheme",
                        Name = "Authorization",
                        In = "header",
                        Required = true,
                        Type = "string",
                        Default = "Bearer "

                    });
            }


           
        }
    }
}
