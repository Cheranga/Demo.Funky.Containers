namespace FunkyContainers.Core
{
    public class Result
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool Status => string.IsNullOrEmpty(ErrorCode);

        public static Result Failure(string errorCode, string errorMessage)
        {
            return new Result
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }

        public static Result Success()
        {
            return new Result();
        }
    }
    
    public class Result<T>
    {
        public T Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public bool Status => string.IsNullOrEmpty(ErrorCode);

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                Data = data
            };
        }

        public static Result<T> Failure(string errorCode, string errorMessage)
        {
            return new Result<T>
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}