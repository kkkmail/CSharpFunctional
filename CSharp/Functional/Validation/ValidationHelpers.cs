namespace CSharp.Lessons.Functional.Validation;

public static class ValidationHelpers
{
    /// <summary>
    /// Disallowes reference types to be nulls and lets any value type values pass.
    /// https://stackoverflow.com/questions/65351/null-or-default-comparison-of-generic-argument-in-c-sharp
    /// </summary>
    public static bool CanNotBeNull<TValue>(this TValue v) =>
            typeof(TValue).IsValueType || !EqualityComparer<TValue>.Default.Equals(v, default(TValue));
}
