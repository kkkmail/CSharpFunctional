namespace CSharp.Lessons.Functional.Validation;

public interface IValidationRule<TValue, TError>
    where TValue : IComparable<TValue>
{
    Result<TValue, TError> Validate(TValue value);
    bool CanAggregate { get; }
}