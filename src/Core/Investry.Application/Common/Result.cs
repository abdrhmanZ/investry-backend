namespace Investry.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public List<Error> Errors { get; private set; }
        public T Value { get; private set; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, List<Error> errors, T value)
        {
            IsSuccess = isSuccess;
            Errors = errors;
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, null, value);
        public static Result<T> Failure(List<Error> errors) => new Result<T>(false, errors, default);
    }
}
