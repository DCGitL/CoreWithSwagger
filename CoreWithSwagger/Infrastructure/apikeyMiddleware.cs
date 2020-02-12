using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CoreWithSwagger.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class apikeyMiddleware
    {
        private readonly RequestDelegate _next;

        public apikeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class apikeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseapikeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<apikeyMiddleware>();
        }
    }
}
