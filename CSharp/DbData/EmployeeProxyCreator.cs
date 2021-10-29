﻿// This looks ugly.
// Unfortunately C# 10 requires fully qualified names here and cannot use already known namespaces.
using EmployeeResult = CSharp.Lessons.Functional.Result<CSharp.Lessons.BusinessEntities.Employee, CSharp.Lessons.Primitives.ErrorData>;

using System.Linq.Expressions;
namespace CSharp.Lessons.DbData;

public static class EmployeeProxyCreator
{
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

    private static EmployeeResult LoadEmployee(
        this ConnectionString c,
        Expression<Func<EFEmployee, bool>> predicate,
        Func<string> error)
    {
        using var ctx = c.GetContext();
        var employee = ctx.EmployeeSet.SingleOrDefault(predicate);
        if (employee == null) return ToInvalidDataError(error);

        var (s, f) = ctx.EmployeeDataSet
            .Where(e => e.EmployeeId == employee.EmployeeId)
            .Select(e => e.MapEmployeeData())
            .UnzipResults();

        if (f.Count > 0) return ToInvalidDataError($"Some data is invalid: {string.Join(", ", f.Select(v => $"{v}"))}");

        var x = employee.MapEmployee()
            .Map(e => e with { Data = s.ToImmutableDictionary(v => v.EmployeeDataType) });

        return x;
    }

    private static IEnumerable<EmployeeResult> LoadSubordinates(this ConnectionString c, EmployeeId employeeId)
    {
        using var ctx = c.GetContext();

        var subordinates = ctx.EmployeeSet
            .Where(e => e.ManagedByEmployeeId == employeeId.Value)
            .Select(e => e.MapEmployee());

        return subordinates;
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

    private static void SaveEmployee(this ConnectionString c, Employee employee)
    {
        using var ctx = c.GetContext();

        ctx.DeleteEmployeeData(employee.EmployeeId);
    }

    private static Func<EmployeeId, EmployeeResult> LoadEmployee(this ConnectionString c) =>
        employeeId => TryDbFun(() => c.LoadEmployee(
            e => e.EmployeeId == employeeId.Value,
            () => $"Cannot find employee with ID: '{employeeId}'."));

    private static Func<EmployeeEmail, EmployeeResult> LoadEmployeeByEmail(this ConnectionString c) =>
        employeeEmail => TryDbFun(() => c.LoadEmployee(
            e => e.EmployeeEmail == employeeEmail.Value,
            () => $"Cannot find employee with email: '{employeeEmail}'."));

    private static Func<Employee, EmployeeResult> SaveEmployee(this ConnectionString c) =>
        null;

    private static Func<EmployeeId, ImmutableList<EmployeeResult>> LoadSubordinates(this ConnectionString c) =>
        employeeId => TryListDbFun<Employee>(() => c.LoadSubordinates(employeeId));

    public static EmployeeProxy CreateEmployeeProxy(this ConnectionString c)
    {
        return new EmployeeProxy(
            LoadEmployee: c.LoadEmployee(),
            LoadEmployeeByEmail: c.LoadEmployeeByEmail(),
            SaveEmployee: c.SaveEmployee(),
            LoadSubordinates: c.LoadSubordinates());
    }
}
