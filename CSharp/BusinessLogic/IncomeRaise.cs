namespace CSharp.Lessons.BusinessLogic;

public record struct IncomeRaise : IIncomeRaise
{
    private IIncomeRaise Value { get; }
    private IncomeRaise(IIncomeRaise value) => Value = value;

    public IncomeRaiseType IncomeRaiseType => Value.IncomeRaiseType;
    public Func<Employee, Employee> RaiseSalary => Value.RaiseSalary;

    public static implicit operator IncomeRaise(IncomeRaiseByPct raiseByPct) =>
        new IncomeRaise(raiseByPct);

    public static implicit operator IncomeRaise(IncomeRaiseByAmount raiseByAmount) =>
        new IncomeRaise(raiseByAmount);

    public IEnumerable<Employee> RaiseAll(IEnumerable<Employee> employees) =>
        employees.Select(Value.RaiseSalary);
}
