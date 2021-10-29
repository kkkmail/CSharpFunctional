namespace CSharp.Lessons.Primitives.Validators;

public record EmailShouldHaveOneAtValidator : ValidationRule<string, ErrorData>
{
    public static Func<string, bool> IsValidImpl { get; } =
        email => email.Count(c => c == '@') == 1;

    public static Func<string, ErrorData> GetErrorImpl { get; } = 
        email => new ErrorData(ErrorMessage: $"Email: '{email}' must have exactly one '@'.");

    public EmailShouldHaveOneAtValidator() : base(IsValidImpl, GetErrorImpl)
    {
    }
}

