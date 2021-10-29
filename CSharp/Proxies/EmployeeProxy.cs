namespace CSharp.Lessons.Proxies;

public record struct EmployeeProxy(
    Func<EmployeeId, Result<Employee, ErrorData>> LoadEmployee,
    Func<EmployeeEmail, Result<Employee, ErrorData>> LoadEmployeeByEmail,
    Func<Employee, Result<Employee, ErrorData>> SaveEmployee,
    Func<EmployeeId, ImmutableList<Result<Employee, ErrorData>>> LoadSubordinates);
