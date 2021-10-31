namespace CSharp.Lessons.Functional;

public record struct Option<T>
{
    private T Value { get; }
    private bool IsSome { get; }
    private bool IsNone => !IsSome;

    private Option(T value)
    {
        if (value == null) throw new ArgumentNullException();
        IsSome = true;
        Value = value;
    }

    public static implicit operator Option<T>(Option.None _) => new Option<T>();
    public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);

    public static implicit operator Option<T>(T value)
        => value == null ? None : Some(value);

    public R Match<R>(Func<R> none, Func<T, R> some) => IsSome ? some(Value) : none();

    public IEnumerable<T> AsEnumerable()
    {
        if (IsSome) yield return Value;
    }

    public override string ToString() => IsSome ? $"Some({Value})" : "None";
}
