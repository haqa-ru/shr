using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using shr.API.Exceptions;
using shr.API.Models;

namespace shr.API.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("error")]
        public Error Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var exception = context?.Error;
            var message = exception?.Message ?? "Internal Server Error";
            var statusCode = exception switch
            {
                ValidationException => 400,
                NotFoundException => 404,
                UnauthorizedException => 401,
                _ => 500
            };

            if (statusCode == 500)
            {
                _logger.LogError(context?.Path, statusCode, message, DateTime.UtcNow.ToLongTimeString());
            }
            else
            {
                _logger.LogWarning(context?.Path, statusCode, message, DateTime.UtcNow.ToLongTimeString());
            }

            Response.StatusCode = statusCode;

            return new Error(statusCode, message);
        }
    }
}
