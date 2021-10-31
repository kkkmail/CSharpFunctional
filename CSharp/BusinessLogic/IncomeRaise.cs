namespace CSharp.Lessons.BusinessLogic;

public record IncomeRaise : OpenSetBase<IncomeRaise, IIncomeRaise, ErrorData>, IIncomeRaise
{
    public IncomeRaise(IIncomeRaise value) : base(value)
    {
    }

    public IncomeRaiseType IncomeRaiseType => Value.IncomeRaiseType;
    public Func<Employee, Employee> RaiseSalary => Value.RaiseSalary;

    public int CompareTo(IIncomeRaise? other) =>
        Comparer<IIncomeRaise>.Default.Compare(this, other);

    public IEnumerable<Employee> RaiseAll(IEnumerable<Employee> employees) =>
        employees.Select(Value.RaiseSalary);
}
