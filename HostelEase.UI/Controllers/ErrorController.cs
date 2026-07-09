using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HostelEase.Application.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace HostelEase.UI.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;

            if (exception != null)
            {
                _logger.LogError(exception, "Unhandled exception occurred");

                switch (exception)
                {
                    case AppException appException:
                        Response.StatusCode = appException.StatusCode;
                        ViewBag.Message = appException.Message;
                        return View(appException.StatusCode.ToString());

                    case SqlException or RetryLimitExceededException:
                        Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        ViewBag.Message = "Database is unavailable. Please try again later.";
                        return View("503");

                    case TimeoutException:
                        Response.StatusCode = StatusCodes.Status408RequestTimeout;
                        ViewBag.Message = "The request timed out. Please try again.";
                        return View("408");

                    case UnauthorizedAccessException:
                        Response.StatusCode = StatusCodes.Status403Forbidden;
                        ViewBag.Message = "You do not have permission to access this resource.";
                        return View("403");

                    default:
                        Response.StatusCode = StatusCodes.Status500InternalServerError;
                        ViewBag.Message = "An unexpected error occurred. Please try again later.";
                        return View("500");
                }
            }

            // Default 500 error if no exception feature
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            ViewBag.Message = "An unexpected error occurred.";
            return View("500");
        }

        [HttpGet]
        [Route("{statusCode:int}")]
        public IActionResult StatusCodeHandler(int statusCode)
        {
            Response.StatusCode = statusCode;
            
            ViewBag.StatusCode = statusCode;
            ViewBag.Message = statusCode switch
            {
                400 => "Bad Request - The request was invalid.",
                401 => "Unauthorized - Please log in.",
                403 => "Forbidden - You don't have permission to access this resource.",
                404 => "Not Found - The page you're looking for doesn't exist.",
                500 => "Internal Server Error - Something went wrong.",
                503 => "Service Unavailable - The server is temporarily unavailable.",
                _ => "An error occurred."
            };

            return View(statusCode.ToString());
        }
    }
}