using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Students.Application.Exceptions;

namespace Students.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync
        (
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken            
        )
        {
            if (exception is ArgumentException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var badRequestDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(badRequestDetails, cancellationToken);

                return true;
            }

            if (exception is NotFoundException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                var details = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
                return true;
            }

            if (exception is ConflictException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

                var details = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
                return true;
            }

            if (exception is DbUpdateException dbUpdateException && 
                dbUpdateException.GetBaseException() is SqlException sqlException &&
                (sqlException.Number == 2601 || sqlException.Number == 2627)) 
            {
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

                var conflictDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "A student with this email already exists."
                };

                await httpContext.Response.WriteAsJsonAsync(conflictDetails, cancellationToken);

                return true;
            }

            _logger.LogError(exception, "An unexpected error occurred.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var serverErrorDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            };

            await httpContext.Response.WriteAsJsonAsync(serverErrorDetails, cancellationToken);

            return true;
        }
    }
}
