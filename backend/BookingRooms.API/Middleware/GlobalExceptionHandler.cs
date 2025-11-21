using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace BookingRooms.API.Middleware;

public class GlobalExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        const int statusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var title = ReasonPhrases.GetReasonPhrase(statusCode) ?? "Server Error";
        
        var detail = environment.IsDevelopment()
            ? exception.ToString()
            : "An unexpected error occurred. Please try again later.";

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: httpContext.TraceIdentifier);

        problemDetails.Extensions["requestPath"] = httpContext.Request.Path.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}