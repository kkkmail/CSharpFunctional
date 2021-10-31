namespace CSharp.Lessons.DbData;

public record ConnectionString : OpenSetBase<ConnectionString, string, ErrorData>
{
    private const string ConnectionStringValue = "Data Source=localhost;Initial Catalog=Example;Integrated Security=SSPI";

    /// <summary>
    /// For simplicity of the example, any values are allowed so far.
    /// </summary>
    public ConnectionString(string value) : base(value)
    {
    }

    public static ConnectionString DefaultValue { get; } = new(ConnectionStringValue);
}
