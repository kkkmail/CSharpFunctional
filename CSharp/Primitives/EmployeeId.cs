namespace CSharp.Lessons.Primitives;

public record EmployeeId : OpenSetBase<EmployeeId, long>
{
    /// <summary>
    /// Any values are allowed.
    /// </summary>
    public EmployeeId(long value) : base(value)
    {
    }

    public static EmployeeId DefaultValue = new(0);
}
