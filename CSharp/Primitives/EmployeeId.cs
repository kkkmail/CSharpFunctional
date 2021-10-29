namespace CSharp.Lessons.Primitives;

public record EmployeeId : OpenSetBase<EmployeeId, long, ErrorData>
{
    /// <summary>
    /// Any values are allowed.
    /// </summary>
    public EmployeeId(long value) : base(value)
    {
    }
}
