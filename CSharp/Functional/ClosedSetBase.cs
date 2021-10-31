namespace CSharp.Lessons.Functional;

public abstract record ClosedSetBase<TSetElement, TValue, TError>
    : SetBase<TSetElement, TValue, TError>
    where TSetElement : ClosedSetBase<TSetElement, TValue, TError>
    where TValue : IComparable<TValue>
{
    protected ClosedSetBase(TValue value) : base(value)
    {
    }

    private static Lazy<ImmutableList<TSetElement>> _allValues = new(() => GetAllValuesImpl());
    public static ImmutableList<TSetElement> GetAllValues() => _allValues.Value;

    public static Lazy<ImmutableDictionary<TValue, TSetElement>> _allValuesDictionary =
        new(() => GetAllValuesImpl().ToImmutableDictionary(e => e.Value));

    public static ImmutableDictionary<TValue, TSetElement> GetAllValuesDictionary() => _allValuesDictionary.Value;

    public static Result<TSetElement, TError> TryGetElement(TValue v, Func<TValue, TError> missing) =>
        GetAllValuesDictionary().TryGetValue(v, out var result) ? result : missing(v);
}
