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

            if (IsUniqueConstraintViolation(exception))
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
        private static bool IsUniqueConstraintViolation(Exception exception)
        {
            if (exception is not DbUpdateException dbUpdateException)
            {
                return false;
            }

            var providerException = dbUpdateException.GetBaseException();
            if (providerException is SqlException sqlException)
            {
                return sqlException.Number is 2601 or 2627;
            }

            return IsSqliteUniqueConstraintViolation(providerException);
        }

        private static bool IsSqliteUniqueConstraintViolation(Exception exception)
        {
            if (exception.GetType().FullName != "Microsoft.Data.Sqlite.SqliteException")
            {
                return false;
            }

            var exceptionType = exception.GetType();
            var errorCodeValue = exceptionType.GetProperty("SqliteErrorCode")?.GetValue(exception);
            var extendedErrorCodeValue = exceptionType.GetProperty("SqliteExtendedErrorCode")?.GetValue(exception);

            return errorCodeValue is int errorCode && errorCode == 19
                && extendedErrorCodeValue is int extendedErrorCode
                && extendedErrorCode is 1555 or 2067;
        }
    }
}
