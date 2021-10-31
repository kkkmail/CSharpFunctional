namespace CSharp.Lessons.BusinessEntities;

public record EmployeeData(
    EmployeeDataType EmployeeDataType,
    Option<string> EmploeeDataValue);
