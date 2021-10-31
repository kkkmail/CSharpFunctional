// Based on: https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Option.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

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
