namespace CSharp.Lessons.BusinessLogic;

public record IncomeRaise : OpenSetBase<IncomeRaise, IIncomeRaise, ErrorData>
{
    public IncomeRaise(IIncomeRaise value) : base(value)
    {
    }

    public ImmutableList<Employee> RaiseAll(IEnumerable<Employee> employees) =>
        employees
            .Select(Value.RaiseSalary)
            .ToImmutableList();
}
