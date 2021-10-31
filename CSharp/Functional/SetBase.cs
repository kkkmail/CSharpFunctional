using System.Reflection;

namespace CSharp.Lessons.Functional;

public abstract record SetBase<TSetElement, TValue, TError>
    where TSetElement : SetBase<TSetElement, TValue, TError>
    where TValue : IComparable<TValue>
{
    public TValue Value { get; }
    protected SetBase(TValue value) => Value = value;

    protected static Func<TValue, Result<TValue, TError>> NoValidation { get; } =
        NoValidation<TValue, TError>();

    protected static Result<TSetElement, TError> TryCreate(
        TValue value,
        Func<TValue, TSetElement> creator,
        Func<TValue, TError> errorCreator,
        Func<TValue, Result<TValue, TError>>? extraValidator = null) =>
        TryCreate<TSetElement, TValue, TError>(value, creator, errorCreator, extraValidator);

    protected static ImmutableList<TSetElement> GetAllValuesImpl(Type? t = null)
    {
        t ??= typeof(TSetElement);
        var values = t.GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(GetAllValuesImpl)
            .Concat(t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(TSetElement))
                .Select(x => x.GetValue(null) as TSetElement)
                .Where(x => x! != null!))
            .Distinct()
            .ToImmutableList();

        return values!;
    }
}
