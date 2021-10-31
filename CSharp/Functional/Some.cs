namespace CSharp.Lessons.Functional;

public record struct Some<T> : IOption<T>
{
    private T Value { get; }
    public R Match<R>(Func<R> none, Func<T, R> some) => some(Value);
    public override string ToString() => $"Some({Value})";
    internal Some(T value) => Value = value;

    public IEnumerable<T> AsEnumerable()
    {
        yield return Value;
    }
}
