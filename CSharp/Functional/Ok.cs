namespace CSharp.Lessons.Functional;

public record struct Ok<TResult, TError> : IResult<TResult, TError>
{
    public bool IsOk => true;
    internal TResult OkResult { get; }
    public Ok(TResult result) => OkResult = result;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => ok(OkResult);
    public override string ToString() => $"Ok({OkResult})";

    public IEnumerator<TResult> AsResultEnumerable()
    {
        yield return OkResult;
    }

    public IEnumerator<TError> AsErrorEnumerable()
    {
        yield break;
    }
}
