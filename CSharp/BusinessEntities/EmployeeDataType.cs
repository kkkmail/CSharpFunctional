namespace CSharp.Lessons.BusinessEntities;

public record EmployeeDataType : ClosedSetBase<EmployeeDataType, long, ErrorData>
{
    public string Name { get; }

    private EmployeeDataType(long value, [CallerMemberName] string name = null!) : base(value) =>
        Name = name;

    public static EmployeeDataType FavoriteRestaurant { get; } = new EmployeeDataType(1);
    public static EmployeeDataType HighSchoolMascot { get; } = new EmployeeDataType(2);
    public static EmployeeDataType PetName { get; } = new EmployeeDataType(3);

    public static Result<EmployeeDataType, ErrorData> TryGetElement(long employeeDataTypeId) =>
        TryGetElement(employeeDataTypeId, e => new ErrorData($"Invalid data: {e}"));
}
