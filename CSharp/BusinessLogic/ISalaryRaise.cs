namespace CSharp.Lessons.BusinessLogic;

public interface ISalaryRaise
{
    SalaryRaiseType SalaryRaiseType { get; }
    Func<Employee, Employee> RaiseSalary { get; }
}
