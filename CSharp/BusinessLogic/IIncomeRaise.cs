namespace CSharp.Lessons.BusinessLogic;

public interface IIncomeRaise
{
    IncomeRaiseType IncomeRaiseType { get; }
    Func<Employee, Employee> RaiseSalary { get; }
}
