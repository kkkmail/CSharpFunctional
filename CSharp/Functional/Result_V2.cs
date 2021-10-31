namespace CSharp.Lessons.Functional;

public record struct Result<TResult, TError> : IResult<TResult, TError>
{
    private IResult<TResult, TError> Value { get; }
    public bool IsOk => Value.IsOk;
    public bool IsError => Value.IsError;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => Value.Match(ok, error);
    public IEnumerator<TError> AsErrorEnumerable() => Value.AsErrorEnumerable();
    public IEnumerator<TResult> AsResultEnumerable() => Value.AsResultEnumerable();
    public override string ToString() => Value.ToString()!;
    private Result(IResult<TResult, TError> result) => Value = result;

    public static implicit operator Result<TResult, TError>(TResult result) => 
        new Result<TResult, TError>(new Ok<TResult, TError>(result));

    public static implicit operator Result<TResult, TError>(TError error) => 
        new Result<TResult, TError>(new Error<TResult, TError>(error));

    public static implicit operator Result<TResult, TError>(Result.Ok<TResult> result) =>
        new Result<TResult, TError>(new Ok<TResult, TError>(result.Value));

    public static implicit operator Result<TResult, TError>(Result.Error<TError> error) =>
        new Result<TResult, TError>(new Error<TResult, TError>(error.Value));
}
