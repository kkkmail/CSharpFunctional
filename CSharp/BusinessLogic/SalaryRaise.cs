namespace CSharp.Lessons.BusinessLogic;

public record struct SalaryRaise : ISalaryRaise
{
    private ISalaryRaise Value { get; }
    private SalaryRaise(ISalaryRaise value) => Value = value;

    public SalaryRaiseType SalaryRaiseType => Value.SalaryRaiseType;
    public Func<Employee, Employee> RaiseSalary => Value.RaiseSalary;

    public static implicit operator SalaryRaise(SalaryRaiseByPct raiseByPct) =>
        new SalaryRaise(raiseByPct);

    public static implicit operator SalaryRaise(SalaryRaiseByAmount raiseByAmount) =>
        new SalaryRaise(raiseByAmount);

    public IEnumerable<Employee> RaiseAll(IEnumerable<Employee> employees) =>
        employees.Select(Value.RaiseSalary);
}
