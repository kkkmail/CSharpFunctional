using System.Reflection;

namespace CSharp.Lessons.Functional;

public abstract record SetBase<TSetElement, TValue>(TValue Value)
    where TSetElement : SetBase<TSetElement, TValue>
    where TValue : IComparable<TValue>
{
    protected static Func<TValue, Result<TValue, TError>> NoValidation<TError>() =>
        NoValidation<TValue, TError>();

    protected static Result<TSetElement, TError> TryCreate<TError>(
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
