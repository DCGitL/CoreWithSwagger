using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MessageManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CoreWithSwagger.ExceptionHandling
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var sendEmail = httpContext.RequestServices.GetService(typeof(IMessageServices)) as MessageSerivces;
            //try
            //{ 
            //before the controller
            var feature1 = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var path1 = feature1?.Path;

            await   _next(httpContext);

            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var path = feature?.Path;
            //after the controller
            //}
            //catch(Exception ex)
            //{
            //    if(sendEmail != null)
            //    {
            //        sendEmail.SendEmail(ex);
            //    }

            //    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //}

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
