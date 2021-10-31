namespace CSharp.Lessons.Functional;

public readonly record struct Ok<TResult, TError> : IResult<TResult, TError>
{
    public bool IsOk => true;
    public bool IsError => !IsOk;
    private TResult Value { get; }
    public Ok(TResult result) => Value = result;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => ok(Value);
    public override string ToString() => $"Ok({Value})";

    public IEnumerator<TResult> AsResultEnumerable()
    {
        yield return Value;
    }

    public IEnumerator<TError> AsErrorEnumerable()
    {
        yield break;
    }
}
