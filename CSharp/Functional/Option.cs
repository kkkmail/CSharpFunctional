// Based on: https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Option.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public record struct Option<T>
{
    readonly T value;
    readonly bool isSome;
    bool isNone => !isSome;

    private Option(T value)
    {
        if (value == null) throw new ArgumentNullException();
        this.isSome = true;
        this.value = value;
    }

    public static implicit operator Option<T>(Option.None _) => new Option<T>();
    public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);

    public static implicit operator Option<T>(T value)
        => value == null ? None : Some(value);

    public R Match<R>(Func<R> none, Func<T, R> some)
        => isSome ? some(value) : none();

    public IEnumerable<T> AsEnumerable()
    {
        if (isSome) yield return value;
    }

    public override string ToString() => isSome ? $"Some({value})" : "None";
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
