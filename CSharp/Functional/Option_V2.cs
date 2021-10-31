namespace CSharp.Lessons.Functional;

public record struct Option<T> : IOption<T>
{
    private IOption<T> Value { get; }
    public R Match<R>(Func<R> none, Func<T, R> some) => Value.Match(none, some);
    public override string ToString() => Value.ToString()!;
    public IEnumerable<T> AsEnumerable() => Value.AsEnumerable();
    private Option(IOption<T> value) => Value = value;

    public static implicit operator Option<T>(Option.None _) => new Option<T>();
    public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(new Some<T>(some.Value));
}
