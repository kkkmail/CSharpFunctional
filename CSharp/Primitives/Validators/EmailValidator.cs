namespace CSharp.Lessons.Primitives.Validators;

public record EmailValidator : AggregateValidationRule<string, ErrorData>
{
    private static ImmutableList<IValidationRule<string, ErrorData>> ValidationRules { get; } =
        new IValidationRule<string, ErrorData>[]
        {
            new EmailShouldNotBeEmptyValidator(),
            new EmailShouldHaveOneAtValidator(),
        }
        .ToImmutableList();

    public EmailValidator() : base(ValidationRules, ErrorData.Combine, false)
    {
    }
}
