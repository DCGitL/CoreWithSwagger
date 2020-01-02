using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Threading.Tasks;


namespace CoreWithSwagger.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<ApiKey> config;


        public HeaderMiddleware(RequestDelegate next, IOptions<ApiKey> _config)
        {
         
            _next = next;
            config = _config;
        }

        public async Task Invoke(HttpContext httpContext)

        {
            IHeaderDictionary headers = httpContext.Request.Headers;
           
            if(headers.TryGetValue("Apikey", out StringValues values))
            {
                var apikeysecret = config.Value.Secret;

                if (StringValues.IsNullOrEmpty(values))
                {
                    
                  // await  httpContext.Response.WriteAsync("Please provide a apikey");
                    httpContext.Response.StatusCode =(int) HttpStatusCode.BadRequest; //.WriteAsync("Please provide a Apikey value");
                    return;
                }
                if(! values[0].Equals(apikeysecret))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation; //.WriteAsync("Please 
                    return;

                }
             
            }
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
