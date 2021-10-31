namespace CSharp.Lessons.Functional;

public record struct Ok<TResult, TError> : IResult<TResult, TError>
{
    public bool IsOk => true;
    internal TResult Result { get; }

    internal Ok(TResult result) => Result = result;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => ok(Result);
}
