﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWithSwagger.ExceptionHandling
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue -10;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception is HttpResponseException exception)
            {
                context.Result = new ObjectResult(exception.Value)
                {
                    StatusCode = exception.Status
                };
                context.ExceptionHandled = true;

            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {}
    }
}
