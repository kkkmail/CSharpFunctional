namespace CSharp.Lessons.Primitives;

public interface IEmail
{
    string Email { get; }
}

public abstract record EmailBase<T> : OpenSetBase<T, string, ErrorData>, IEmail
    where T : OpenSetBase<T, string, ErrorData>
{
    public string Email => Value;

    /// <summary>
    /// To hide Value from outside world.
    /// </summary>
    private new string Value => base.Value;

    protected EmailBase(string value) : base(value)
    {
    }

    private static Func<string, string> Standardizer { get; } =
        s => s.ToLower().Trim();

    private static Func<string, Result<string, ErrorData>> Validator { get; } =
        new EmailValidator().Validate;

    protected static Result<T, ErrorData> TryCreate(
        string email,
        Func<string, T> creator,
        Func<string, Result<string, ErrorData>>? extraValidator = null) =>
        TryCreate(
            email,
            Standardizer.Compose(e => creator(e)),
            _ => ErrorData.ValueCannotBeNull<string>(typeof(T).Name),
            Validator.Compose(r => r.Bind(extraValidator ?? NoValidation)));
}
