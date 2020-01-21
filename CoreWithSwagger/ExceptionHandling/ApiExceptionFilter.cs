using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.ExceptionHandling
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            ProblemDetails problem = null;
            var feature = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if(context.Exception is UnauthorizedAccessException )
            {
                problem = new ProblemDetails
                {
                    Title = "User is unauthorized",
                    Status = 401,
                    Instance = feature?.Path,
                    Detail = context.Exception.Message

                };
            }
            else
            {
                problem = new ProblemDetails
                {
                    Title = "An unhandled server error occurred",
                    Status = 500,
                    Instance = feature?.Path,
                    Detail = context.Exception.StackTrace
                };
            }

            context.Result = new JsonResult(problem);

            await base.OnExceptionAsync(context);
        }
    }
}
