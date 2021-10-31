namespace CSharp.Lessons.Functional;

public static class OptionExt2
{
    /// <summary>
    /// For database interop only!
    /// </summary>
    public static Option<T> ToOption<T>(this T? value)
        where T : struct => value != null ? Some(value.Value) : None;
}
