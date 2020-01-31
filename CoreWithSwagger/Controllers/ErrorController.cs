using MessageManager;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace CoreWithSwagger.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IMessageServices messageServices;
        private readonly ILogger<ErrorController> logger;

        public ErrorController(IMessageServices _messageServices, ILogger<ErrorController> logger)
        {
            this.messageServices = _messageServices;
            this.logger = logger;
        }
        [Route("/error")]
      
        public IActionResult Error([FromServices] IHostingEnvironment webHostEnvironment)
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            var isDev = webHostEnvironment.IsDevelopment();
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = feature?.Path,
                Title = isDev ? $"{ex.GetType().Name} : {ex.Message}" : "An error occured.",
                Detail = isDev ? ex.StackTrace : null,
            };
            messageServices.SendEmail(ex);
            logger.LogError(ex, $"Unhandled exception occured at: {DateTime.Now}", null);

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }

        [Route("/error-local-development")]
        
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IHostingEnvironment webHostEnvironment)
        {
            if (!webHostEnvironment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = feature?.Path,
                Title = ex.GetType().Name,
                Detail = ex.StackTrace,
            };
            logger.LogError(ex, $"Unhandled exception occured at: {DateTime.Now}", null);
          //  messageServices.SendEmail(ex);

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }
    }
}