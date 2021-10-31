namespace CSharp.Lessons.BusinessLogic;

public interface IIncomeRaise : IComparable<IIncomeRaise>
{
    IncomeRaiseType IncomeRaiseType { get; }
    Func<Employee, Employee> RaiseSalary { get; }
}
