// Based on: https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Option.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public readonly record struct Option<T> : IOption<T>
{
    private IOption<T> Value { get; }
    public R Match<R>(Func<R> none, Func<T, R> some) => Value.Match(none, some);
    public override string ToString() => Value.ToString()!;
    public IEnumerable<T> AsEnumerable() => Value.AsEnumerable();
    private Option(IOption<T> value) => Value = value;

    public static implicit operator Option<T>(Option.None _) => new Option<T>(new None<T>());
    public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(new Some<T>(some.Value));
}

public static class Option
{
    public record struct None
    {
        internal static None Default { get; } = new None();
    }

    public record struct Some<T>
    {
        internal T Value { get; }

        internal Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value)
                    , "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }
}
