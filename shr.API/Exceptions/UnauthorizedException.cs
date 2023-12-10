using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shr.API.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public const string DEFAULT_MESSAGE = "Unauthorized";

        public UnauthorizedException() : base(DEFAULT_MESSAGE) { }

        public UnauthorizedException(string message) : base(message) { }
    }
}
