using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shr.API.Exceptions
{
    public class ValidationException : Exception
    {
        public const string DEFAULT_MESSAGE = "Validation Error";

        public override string Message { get; }

        public ValidationException() : base()
        {
            Message = DEFAULT_MESSAGE;
        }

        public ValidationException(string message) : base()
        {
            Message = message;
        }

        public ValidationException(ModelStateDictionary modelState) : base()
        {
            Message = modelState
                .FirstOrDefault(x => x.Value is not null && x.Value.Errors.Any())
                .Value?.Errors?.FirstOrDefault()?.ErrorMessage ?? String.Empty;
        }
    }
}
