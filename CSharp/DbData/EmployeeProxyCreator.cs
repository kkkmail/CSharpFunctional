// This looks ugly.
// Unfortunately C# 10 requires fully qualified names here and cannot use already known namespaces.
using EmployeeResult = CSharp.Lessons.Functional.Result<CSharp.Lessons.BusinessEntities.Employee, CSharp.Lessons.Primitives.ErrorData>;

using System.Linq.Expressions;
namespace CSharp.Lessons.DbData;

public static class EmployeeProxyCreator
{

    #region Private Methods

    private static DatabaseContext GetContext(this ConnectionString c) =>
        new DatabaseContext(c);

    private static ErrorData MapExceptionToError(Exception e) =>
        new ErrorData($"Exception: '{e}'.");

    private static ErrorData ToInvalidDataError(string s) =>
        new ErrorData($"Some data is invalid: '{s}'.");

    private static ErrorData ToInvalidDataError(Func<string> e) =>
        ToInvalidDataError(e());

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

    private static ImmutableList<Result<T, ErrorData>> TryListDbFun<T>(Func<IEnumerable<Result<T, ErrorData>>> f)
    {
        try
        {
            return f().ToImmutableList();
        }
        catch (Exception e)
        {
            return ((Result<T, ErrorData>)Error(MapExceptionToError(e))).ToImmutableList();
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

    private static Result<Option<Employee>, ErrorData> LoadEmployee(
        this ConnectionString c,
        Expression<Func<EFEmployee, bool>> predicate)
    {
        using var ctx = c.GetContext();
        var employee = ctx.EmployeeSet.SingleOrDefault(predicate);
        if (employee == null) return Ok((Option<Employee>)None);

        var (s, f) = ctx.EmployeeDataSet
            .Where(e => e.EmployeeId == employee.EmployeeId)
            .Select(e => e.MapEmployeeData())
            .UnzipResults();

        if (f.Count > 0) return ToInvalidDataError($"Some data is invalid: {string.Join(", ", f.Select(v => $"{v}"))}");

        var x = employee.MapEmployee()
            .Map(e => e with { Data = s.ToImmutableDictionary(v => v.EmployeeDataType) })
            .Map(e => Some(e));

        return x;
    }

    private static IEnumerable<EmployeeResult> LoadSubordinates(this ConnectionString c, EmployeeId employeeId)
    {
        using var ctx = c.GetContext();

        var subordinates = ctx.EmployeeSet
            .Where(e => e.ManagedByEmployeeId == employeeId.Value)
            .Select(e => e.MapEmployee())
            .ToList();

        return subordinates;
    }

    private static IEnumerable<EmployeeResult> LoadAllImpl(this ConnectionString c)
    {
        using var ctx = c.GetContext();

        var employees = ctx.EmployeeSet
            .Select(e => e.EmployeeId)
            .ToList()
            .Select(v => new { EmployeeId = new EmployeeId(v), Employee = c.LoadEmployee(e => e.EmployeeId == v) })
            .Select(e => e.Employee.MapOption(e.EmployeeId.ToMissignEmployeeError));

        return employees;
    }

    private static void DeleteEmployeeData(this DatabaseContext ctx, EmployeeId i)
    {
        ctx.EmployeeDataSet.RemoveRange(ctx.EmployeeDataSet.Where(e => e.EmployeeId == i.Value));
    }

    private static void SaveEmployeeData(this DatabaseContext ctx, EmployeeId i, EmployeeData data)
    {
        var dt = ctx.EmployeeDataTypeSet
            .Where(e => e.EmployeeDataTypeId == data.EmployeeDataType.Value)
            .SingleOrDefault();

        if (dt == null)
        {
            ctx.EmployeeDataTypeSet.Add(new EFEmployeeDataType
            {
                EmployeeDataTypeId = data.EmployeeDataType.Value,
                EmployeeDataTypeName = data.EmployeeDataType.Name,
            });
        }

        ctx.EmployeeDataSet.Add(new EFEmployeeData
        {
            EmployeeId = i.Value,
            EmployeeDataTypeId = data.EmployeeDataType.Value,
            EmployeeDataValue = data.EmploeeDataValue.GetOrElse((string?)null!)
        });
    }

    private static EmployeeResult SaveEmployee(this ConnectionString c, Employee employee)
    {
        using var ctx = c.GetContext();
        using var trn = ctx.Database.BeginTransaction();
        ctx.DeleteEmployeeData(employee.EmployeeId);

        void update(EFEmployee e)
        {
            e.EmployeeName = employee.EmployeeName.Value;
            e.EmployeeEmail = employee.EmployeeEmail.Value;
            e.ManagedByEmployeeId = employee.ManagedBy.Map(v => v.Value).FromOption();
            e.DateHired = employee.DateHired;
            e.Salary = employee.Salary;
            e.Description = employee.Description.FromOption();
        }

        var e = ctx.EmployeeSet.SingleOrDefault(e => e.EmployeeId == employee.EmployeeId.Value);

        if (e == null)
        {
            e = new EFEmployee();
            update(e);
            ctx.EmployeeSet.Add(e);
        }
        else
        {
            update(e);
        }

        ctx.SaveChanges();
        var employeeId = new EmployeeId(e.EmployeeId);

        foreach (var data in employee.Data)
        {
            ctx.SaveEmployeeData(employeeId, data.Value);
        }

        ctx.SaveChanges();
        trn.Commit();

        var x = c.LoadEmployee()(employeeId);
        return x;
    }

    private static ErrorData ToMissignEmployeeError(this EmployeeId employeeId) =>
        ToInvalidDataError(() => $"Cannot find employee with ID: '{employeeId}'.");

    private static Func<EmployeeId, EmployeeResult> LoadEmployee(this ConnectionString c) =>
        employeeId => TryDbFun(() => c.LoadEmployee(e => e.EmployeeId == employeeId.Value))
            .MapOption(employeeId.ToMissignEmployeeError);

    private static Func<EmployeeEmail, Result<Option<Employee>, ErrorData>> LoadEmployeeByEmail(this ConnectionString c) =>
        employeeEmail => TryDbFun(() => c.LoadEmployee(e => e.EmployeeEmail == employeeEmail.Value));

    private static Func<Employee, EmployeeResult> SaveEmployee(this ConnectionString c) =>
        employee => TryDbFun(() => c.SaveEmployee(employee));

    private static Func<EmployeeId, ImmutableList<EmployeeResult>> LoadSubordinates(this ConnectionString c) =>
        employeeId => TryListDbFun<Employee>(() => c.LoadSubordinates(employeeId));

    private static Func<ImmutableList<EmployeeResult>> LoadAll(this ConnectionString c) =>
        () => TryListDbFun<Employee>(() => c.LoadAllImpl());

    #endregion

    #region Public Methods

    public static EmployeeProxy CreateEmployeeProxy(this ConnectionString c)
    {
        return new EmployeeProxy(
            LoadEmployee: c.LoadEmployee(),
            LoadEmployeeByEmail: c.LoadEmployeeByEmail(),
            SaveEmployee: c.SaveEmployee(),
            LoadSubordinates: c.LoadSubordinates(),
            LoadAll: c.LoadAll());
    }

    #endregion
}
