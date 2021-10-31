namespace CSharp.Lessons.Functional;

public record struct Error<TResult, TError> : IResult<TResult, TError>
{
    public bool IsOk => false;
    internal TError ErrorResult { get; }
    public Error(TError error) => ErrorResult = error;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => error(ErrorResult);
    public override string ToString() => $"Error({ErrorResult})";

    public IEnumerator<TResult> AsResultEnumerable()
    {
        yield break;
    }

    public IEnumerator<TError> AsErrorEnumerable()
    {
        yield return ErrorResult;
    }
}
