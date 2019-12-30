using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;


namespace CoreWithSwagger.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMiddleware(RequestDelegate next)
        {
         
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)

        {
            IHeaderDictionary headers = httpContext.Request.Headers;
            var uril1 = httpContext.Request.GetDisplayUrl();
           
           
             await _next.Invoke(httpContext);
        }

       


       
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderMiddleware>();
        }
    }
}
