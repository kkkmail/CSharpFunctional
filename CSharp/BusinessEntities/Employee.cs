namespace CSharp.Lessons.BusinessEntities;

public record Employee(
    EmployeeId EmployeeId,
    EmployeeName EmployeeName,
    EmployeeEmail EmployeeEmail,
    Option<EmployeeId> ManagedBy,
    DateTime DateHired,
    decimal Salary,
    Option<string> Description,
    ImmutableDictionary<EmployeeDataType, EmployeeData> Data);
