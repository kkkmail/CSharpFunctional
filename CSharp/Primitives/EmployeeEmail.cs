namespace CSharp.Lessons.Primitives;

public record EmployeeEmail : EmailBase<EmployeeEmail>
{
    private EmployeeEmail(string value) : base(value)
    {
    }

    private const string CorporateDomain = "@abcdef.gg";

    public static Func<string, Result<string, ErrorData>> EmployeeEmailValidator { get; } =
        v => v.ToLower().Trim().EndsWith(CorporateDomain)
            ? v
            : new ErrorData("Employee email must end with corporate domain name.");

    public static Result<EmployeeEmail, ErrorData> TryCreate(
        string email,
        Func<string, Result<string, ErrorData>>? extraValidator = null) =>
        TryCreate(
            email,
            e => new EmployeeEmail(e),
            EmployeeEmailValidator.Compose(r => r.Bind(extraValidator ?? NoValidation)));
}
