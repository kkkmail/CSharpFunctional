namespace CSharp.Lessons.Functional;

public static class OptionExt2
{
    /// <summary>
    /// For database interop only!
    /// </summary>
    public static Option<T> ToOption<T>(this T? value)
        where T : struct => value != null ? Some(value.Value) : None;

    /// <summary>
    /// For database interop only!
    /// </summary>
    public static T? FromOption<T>(this Option<T> value) where T : struct =>
        value.AsEnumerable()
            .Select(e => (T?)e)
            .FirstOrDefault();

    /// <summary>
    /// For database interop only!
    /// </summary>
    public static TValue? FromOption<TSetElement, TValue>(this Option<TSetElement> option)
        where TSetElement : SetBase<TSetElement, TValue>
        where TValue : struct, IComparable<TValue> =>
        option.Map(x => x.Value).FromOption();
}
