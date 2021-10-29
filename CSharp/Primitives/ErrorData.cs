namespace CSharp.Lessons.Primitives;

/// <summary>
/// I don't want to go into a rabbit hole here for the time being...
/// </summary>
public record ErrorData(string ErrorMessage)
{
    public static ErrorData ValueCannotBeNull<TValue>(string className) =>
         new ErrorData($"Value of {className}, type: {typeof(TValue)} cannot be null.");

    public static Func<ErrorData, ErrorData, ErrorData> Combine { get; } =
        (a, b) => new ErrorData($"{a.ErrorMessage}, {b.ErrorMessage}");
}
