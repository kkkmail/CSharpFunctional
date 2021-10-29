using System.Reflection;

namespace CSharp.Lessons.Functional;

public abstract record SetBase<TSetElement, TValue, TError>
    where TSetElement : SetBase<TSetElement, TValue, TError>
    where TValue : IComparable<TValue>
{
    public TValue Value { get; }
    protected SetBase(TValue value) => Value = value;
    public static Func<TValue, Result<TValue, TError>> NoValidation { get; } = v => Ok(v);
    public string ClassName => GetType().Name;

    private static Func<TValue, Result<TValue, TError>> CanNotBeNull(Func<TValue, TError> errorCreator) =>
        v => v.CanNotBeNull() ? Ok(v) : errorCreator(v);

    protected static Result<TSetElement, TError> TryCreate(
        TValue value,
        Func<TValue, TSetElement> creator,
        Func<TValue, TError> errorCreator,
        Func<TValue, Result<TValue, TError>>? extraValidator = null) =>
        CanNotBeNull(errorCreator).Compose(r => r.Bind(extraValidator ?? NoValidation))(value).Map(creator);

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
