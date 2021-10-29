namespace CSharp.Lessons.Primitives;

public record EmployeeName : OpenSetBase<EmployeeName, string, ErrorData>
{
    private EmployeeName(string value) : base(value)
    {
    }

    public static Func<string, string> Standardizer { get; } =
        s => s.ToUpper().Trim();

    public static Func<string, Result<string, ErrorData>> Validator { get; } =
        v => v;

    public static Result<EmployeeName, ErrorData> TryCreate(
        string name,
        Func<string, Result<string, ErrorData>>? validator = null) =>
        TryCreate(
            name,
            Standardizer.Compose(n => new EmployeeName(n)),
            _ => ErrorData.ValueCannotBeNull<string>(nameof(EmployeeName)),
            Validator.Compose(r => r.Bind(validator ?? NoValidation)));
}
