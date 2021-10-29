namespace CSharp.Lessons.BusinessEntities;

public abstract record IncomeRaiseBase(Func<Employee, Employee> RaiseSalary);
