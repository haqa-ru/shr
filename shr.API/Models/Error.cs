namespace shr.API.Models {
    public class Error {
        public int StatusCode { get; }
        public string Message { get; }

        public Error(int statusCode, string message) {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
