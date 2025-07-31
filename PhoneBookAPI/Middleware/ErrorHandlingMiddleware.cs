using Microsoft.EntityFrameworkCore;
using PhonebookApi.DTOs;
using PhonebookApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace PhonebookApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                // Custom exceptions
                CustomException customEx => CreateErrorResponse(
                    customEx.Message,
                    customEx.ErrorType,
                    (int)customEx.StatusCode,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    customEx
                ),

                // FluentValidation exceptions
                FluentValidation.ValidationException fluentValidationEx => CreateValidationErrorResponse(
                    "One or more validation errors occurred.",
                    "ValidationError",
                    (int)HttpStatusCode.UnprocessableEntity,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    ConvertFluentValidationErrors(fluentValidationEx),
                    fluentValidationEx
                ),

                // Entity Framework exceptions
                DbUpdateException dbEx => CreateErrorResponse(
                    "A database error occurred while processing your request.",
                    "DatabaseError",
                    (int)HttpStatusCode.InternalServerError,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    dbEx
                ),

                // Argument exceptions
                ArgumentNullException argNullEx => CreateErrorResponse(
                    "A required parameter was not provided.",
                    "BadRequest",
                    (int)HttpStatusCode.BadRequest,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    argNullEx
                ),

                ArgumentException argEx => CreateErrorResponse(
                    "Invalid argument provided.",
                    "BadRequest",
                    (int)HttpStatusCode.BadRequest,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    argEx
                ),

                // Unauthorized access
                UnauthorizedAccessException unauthorizedEx => CreateErrorResponse(
                    "You are not authorized to access this resource.",
                    "Unauthorized",
                    (int)HttpStatusCode.Unauthorized,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    unauthorizedEx
                ),

                // Not implemented
                NotImplementedException notImplEx => CreateErrorResponse(
                    "This feature is not yet implemented.",
                    "NotImplemented",
                    (int)HttpStatusCode.NotImplemented,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    notImplEx
                ),

                // Timeout exceptions
                TimeoutException timeoutEx => CreateErrorResponse(
                    "The request timed out. Please try again later.",
                    "Timeout",
                    (int)HttpStatusCode.RequestTimeout,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    timeoutEx
                ),

                // Generic exceptions
                _ => CreateErrorResponse(
                    "An unexpected error occurred while processing your request.",
                    "InternalServerError",
                    (int)HttpStatusCode.InternalServerError,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    exception
                )
            };

            response.StatusCode = errorResponse.StatusCode;

            // Serialize and write response
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await response.WriteAsync(jsonResponse);
        }

        private ErrorResponseDto CreateErrorResponse(string message, string errorType, int statusCode,
            string path, string method, string traceId, Exception exception)
        {
            if (_environment.IsDevelopment())
            {
                // Development environment - include stack trace and inner exception details
                return new ErrorResponseDto(
                    message,
                    errorType,
                    statusCode,
                    path,
                    method,
                    traceId,
                    exception.StackTrace ?? string.Empty,
                    exception.InnerException?.Message
                );
            }
            else
            {
                // Production environment - minimal error info
                return new ErrorResponseDto(message, errorType, statusCode, path, method, traceId);
            }
        }

        private ErrorResponseDto CreateValidationErrorResponse(string message, string errorType, int statusCode,
            string path, string method, string traceId, Dictionary<string, string[]> errors, Exception exception)
        {
            if (_environment.IsDevelopment())
            {
                return new ErrorResponseDto(
                    message,
                    errorType,
                    statusCode,
                    path,
                    method,
                    traceId,
                    errors
                );
            }
            else
            {
                return new ErrorResponseDto(message, errorType, statusCode, path, method, traceId, errors);
            }
        }

        private Dictionary<string, string[]> ConvertFluentValidationErrors(FluentValidation.ValidationException validationException)
        {
            return validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }
    }

    // Extension method to register middleware
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}