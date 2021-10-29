// This looks ugly.
// Unfortunately C# 10 requires fully qualified names here and cannot use already known namespaces.
using EmployeeResult = CSharp.Lessons.Functional.Result<CSharp.Lessons.BusinessEntities.Employee, CSharp.Lessons.Primitives.ErrorData>;

namespace CSharp.Lessons.DbData;

public static class EmployeeProxyCreator
{

    internal static DatabaseContext GetContext(this ConnectionString c) =>
        new DatabaseContext(c);

    private static ErrorData MapExceptionToError(Exception e) =>
        new ErrorData($"Exception: '{e}'.");

    private static ErrorData ToInvalidDataError(string s) =>
        new ErrorData($"Some data is invalid: '{s}'.");

    private static Result<T, ErrorData> TryDbFun<T>(Func<Result<T, ErrorData>> f)
    {
        try
        {
            return f();
        }
        catch (Exception e)
        {
            return MapExceptionToError(e);
        }
    }

    private static EmployeeResult MapEmployee(this EFEmployee employee)
    {
        var name = EmployeeName.TryCreate(employee.EmployeeName);
        var email = EmployeeEmail.TryCreate(employee.EmployeeEmail);

        return name.Match(
            n => email.Match(
                e => (EmployeeResult)new Employee(
                    EmployeeId: new EmployeeId(employee.EmployeeId),
                    EmployeeName: n,
                    EmployeeEmail: e,
                    ManagedBy: employee.ManagedByEmployeeId.ToOption().Map(m => new EmployeeId(m)),
                    DateHired: employee.DateHired,
                    Salary: employee.Salary,
                    Description: employee.Description.ToOption(),
                    Data: ImmutableDictionary<EmployeeDataType, EmployeeData>.Empty),
                err => err),
            err => err);
    }

    private static Result<EmployeeData, ErrorData> MapEmployeeData(this EFEmployeeData employeeData) =>
        EmployeeDataType.TryGetElement(employeeData.EmployeeDataTypeId)
            .Map(e => new EmployeeData(
                EmployeeDataType: e,
                EmploeeDataValue: employeeData.EmployeeDataValue.ToOption()));

    private static EmployeeResult LoadEmployeeImpl(ConnectionString c, EmployeeId employeeId)
    {
        using var ctx = c.GetContext();
        var employee = ctx.EmployeeSet.SingleOrDefault(e => e.EmployeeId == employeeId.Value);
        if (employee == null) return ToInvalidDataError($"Cannot find employee with ID: '{employeeId}'.");

        var (s, f) = ctx.EmployeeDataSet
            .Where(e => e.EmployeeId == employee.EmployeeId)
            .Select(e => e.MapEmployeeData())
            .UnzipResults();

        if (f.Count > 0) return ToInvalidDataError($"Some data is invalid: {string.Join(", ", f.Select(v => $"{v}"))}");

        var x = employee.MapEmployee()
            .Map(e => e with { Data = s.ToImmutableDictionary(v => v.EmployeeDataType) });

        return x;
    }

    private static Func<EmployeeId, EmployeeResult> LoadEmployee(this ConnectionString c) =>
        employeeId => TryDbFun(() => LoadEmployeeImpl(c, employeeId));

    private static Func<EmployeeEmail, EmployeeResult> LoadEmployeeByEmail(this ConnectionString c) =>
        null;

    private static Func<Employee, EmployeeResult> SaveEmployee(this ConnectionString c) =>
        null;

    private static Func<EmployeeId, ImmutableList<EmployeeResult>> LoadSubordinates(this ConnectionString c) =>
        null;

    public static EmployeeProxy CreateEmployeeProxy(this ConnectionString c)
    {
        return new EmployeeProxy(
            LoadEmployee: c.LoadEmployee(),
            LoadEmployeeByEmail: c.LoadEmployeeByEmail(),
            SaveEmployee: c.SaveEmployee(),
            LoadSubordinates: c.LoadSubordinates());
    }
}
