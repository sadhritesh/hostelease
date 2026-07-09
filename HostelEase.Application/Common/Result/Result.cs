

namespace HostelEase.Application.Common.Result
{
    public class Result
    {
        public bool isSuccess {  get; set; }
        public string? Message { get; set; }
        public IEnumerable<string> Errors { get; set; } = default!;

        public static Result Success(string? message = null)
        {
            return new Result { isSuccess = true, Message = message };
        }

        public static Result Failure(IEnumerable<string> Errors)
        {
            return new Result { isSuccess = false, Errors = Errors };
        }
        public static Result<T> Success<T>(T value, string? message = null) 
        {
            return new Result<T>
            {
                isSuccess = true,
                Value = value,
                Message = message
            };
        }

        public static Result<T> Failure<T>(IEnumerable<string> Errors)
        {
            return new Result<T> 
            { 
                isSuccess = false, 
                Errors = Errors 
            };
        }

    }

    public class Result<T>: Result
    {
        public T? Value { get; set; }
    }
}
