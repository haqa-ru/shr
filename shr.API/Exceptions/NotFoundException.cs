using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shr.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public const string DEFAULT_MESSAGE = "Not Found";

        public NotFoundException() : base(DEFAULT_MESSAGE) { }

        public NotFoundException(string message) : base(message) { }
    }
}
