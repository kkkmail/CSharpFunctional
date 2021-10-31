namespace CSharp.Lessons.Functional;

public record struct Error<TResult, TError> : IResult<TResult, TError>
{
    public bool IsOk => false;
    public bool IsError => !IsOk;
    private TError Value { get; }
    public Error(TError error) => Value = error;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => error(Value);
    public override string ToString() => $"Error({Value})";

    public IEnumerator<TResult> AsResultEnumerable()
    {
        yield break;
    }

    public IEnumerator<TError> AsErrorEnumerable()
    {
        yield return Value;
    }
}
