namespace CSharp.Lessons.Functional.Validation;

public static class ValidationHelpers
{
    /// <summary>
    /// Disallowes reference types to be nulls and lets any value type values pass.
    /// </summary>
    public static bool CanNotBeNull<TValue>(this TValue v) => v != null;
}
