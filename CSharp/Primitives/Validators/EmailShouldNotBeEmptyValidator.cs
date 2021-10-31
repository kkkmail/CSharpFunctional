namespace CSharp.Lessons.Primitives.Validators;

public record EmailShouldNotBeEmptyValidator : ValidationRule<string, ErrorData>
{
    public static Func<string, bool> IsValidImpl { get; } =
        email => !string.IsNullOrWhiteSpace(email);

    public static Func<string, ErrorData> GetErrorImpl { get; } = 
        email => new ErrorData(ErrorMessage: $"Email: '{email}' must not be null or empty.");

    public EmailShouldNotBeEmptyValidator() : base(IsValidImpl, GetErrorImpl, false)
    {
    }
}

