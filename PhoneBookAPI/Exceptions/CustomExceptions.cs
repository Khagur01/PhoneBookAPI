using System.Net;

namespace PhonebookApi.Exceptions
{
    // Base custom exception
    public abstract class CustomException : Exception
    {
        public abstract HttpStatusCode StatusCode { get; }
        public abstract string ErrorType { get; }

        protected CustomException(string message) : base(message) { }
        protected CustomException(string message, Exception innerException) : base(message, innerException) { }
    }

    // 404 Not Found
    public class NotFoundException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public override string ErrorType => "NotFound";

        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with key '{key}' was not found.") { }
    }

    // 400 Bad Request
    public class BadRequestException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public override string ErrorType => "BadRequest";

        public BadRequestException(string message) : base(message) { }
    }

    // 409 Conflict
    public class ConflictException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public override string ErrorType => "Conflict";

        public ConflictException(string message) : base(message) { }
        public ConflictException(string resourceName, object key)
            : base($"{resourceName} with key '{key}' already exists.") { }
    }

    // 422 Unprocessable Entity
    public class ValidationException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
        public override string ErrorType => "ValidationError";
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }

    // 500 Internal Server Error
    public class InternalServerException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
        public override string ErrorType => "InternalServerError";

        public InternalServerException(string message) : base(message) { }
        public InternalServerException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // Database related exceptions
    public class DatabaseException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
        public override string ErrorType => "DatabaseError";

        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // Business logic exceptions
    public class BusinessLogicException : CustomException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public override string ErrorType => "BusinessLogicError";

        public BusinessLogicException(string message) : base(message) { }
    }
}