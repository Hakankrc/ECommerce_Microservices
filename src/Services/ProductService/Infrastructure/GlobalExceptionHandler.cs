using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Infrastructure;

/// <summary>
/// Centralized exception handling middleware.
/// Catch exceptions and converts them into standard HTTP ProblemDetails responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;

        // CASE 1: Validation Errors (Client Side Issue - 400 Bad Request)
        if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation Failed";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = "One or more validation errors occurred.";
            
            // Map validation errors to a structured format
            problemDetails.Extensions["errors"] = validationException.Errors.Select(e => new 
            { 
                Field = e.PropertyName, 
                Message = e.ErrorMessage 
            });

            _logger.LogWarning("⚠️ Validation failed: {Errors}", string.Join(", ", validationException.Errors.Select(e => e.ErrorMessage)));
        }
        // CASE 2: Internal Server Errors (Server Side Issue - 500 Internal Server Error)
        else
        {
            problemDetails.Title = "Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = exception.Message;

            _logger.LogError(exception, "❌ Unhandled exception occurred: {Message}", exception.Message);
        }

        // Write the JSON response
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}