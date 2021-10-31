namespace CSharp.Lessons.Functional.Validation;

public abstract record ValidationRule<TValue, TError>(
    Func<TValue, bool> IsValid,
    Func<TValue, TError> GetError,
    bool CanAggregate = true) : IValidationRule<TValue, TError>
    where TValue : IComparable<TValue>
{
    public Result<TValue, TError> Validate(TValue value) =>
        IsValid(value) ? Ok(value) : GetError(value);
}
