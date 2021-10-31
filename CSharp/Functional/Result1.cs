namespace CSharp.Lessons.Functional;

public record struct Result1<TResult, TError>
{
    internal IResult<TResult, TError> Result { get; }
    internal Result1(IResult<TResult, TError> result) => Result = result;

    public static implicit operator Result1<TResult, TError>(TResult result) => 
        new Result1<TResult, TError>(new Ok<TResult, TError>(result));

    //public static implicit operator Result<TResult, TError>(TError error) => new Result<TResult, TError>(error);

    //public static implicit operator Result<TResult, TError>(Result.Ok<TResult> result) => new Result<TResult, TError>(result.Value);
    //public static implicit operator Result<TResult, TError>(Result.Error<TError> error) => new Result<TResult, TError>(error.Value);

    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => Result.Match(ok, error);
}
