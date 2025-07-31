using System.Text.Json.Serialization;

namespace PhonebookApi.DTOs
{
    public class ErrorResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = false;

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errorType")]
        public string ErrorType { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }

        [JsonPropertyName("stackTrace")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StackTrace { get; set; }

        [JsonPropertyName("innerException")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InnerException { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }

        // Constructor for general errors
        public ErrorResponseDto(string message, string errorType, int statusCode, string path, string method, string traceId)
        {
            Message = message;
            ErrorType = errorType;
            StatusCode = statusCode;
            Path = path;
            Method = method;
            TraceId = traceId;
        }

        // Constructor for validation errors
        public ErrorResponseDto(string message, string errorType, int statusCode, string path, string method, string traceId, Dictionary<string, string[]> errors)
            : this(message, errorType, statusCode, path, method, traceId)
        {
            Errors = errors;
        }

        // Constructor for development environment (with stack trace)
        public ErrorResponseDto(string message, string errorType, int statusCode, string path, string method, string traceId, string stackTrace, string? innerException = null)
            : this(message, errorType, statusCode, path, method, traceId)
        {
            StackTrace = stackTrace;
            InnerException = innerException;
        }
    }

    // Success response wrapper (bonus)
    public class SuccessResponseDto<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } = 200;

        public SuccessResponseDto(T data, string message = "Success")
        {
            Data = data;
            Message = message;
        }
    }

    // API Response wrapper
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Success response
        public static ApiResponse<T> SuccessResult(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        // Error response
        public static ApiResponse<T> ErrorResult(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}