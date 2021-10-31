namespace CSharp.Lessons.Functional;

public readonly record struct Some<T> : IOption<T>
{
    private T Value { get; }
    public T1 Match<T1>(Func<T1> none, Func<T, T1> some) => some(Value);
    public override string ToString() => $"Some({Value})";
    internal Some(T value) => Value = value;

    public IEnumerable<T> AsEnumerable()
    {
        yield return Value;
    }
}
