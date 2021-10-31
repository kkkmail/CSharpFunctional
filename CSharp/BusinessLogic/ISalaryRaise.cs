namespace CSharp.Lessons.BusinessLogic;

public interface ISalaryRaise
{
    SalaryRaiseType IncomeRaiseType { get; }
    Func<Employee, Employee> RaiseSalary { get; }
}
