global using System;
global using System.Linq;
global using MoreLinq.Extensions;
global using System.Collections.Immutable;
global using System.Runtime.CompilerServices;
global using System.Threading.Tasks;

global using CSharp.Lessons;
global using CSharp.Lessons.Functional;
global using CSharp.Lessons.Functional.Validation;
global using CSharp.Lessons.Primitives;
global using CSharp.Lessons.Primitives.Validators;
global using CSharp.Lessons.BusinessEntities;
global using CSharp.Lessons.Proxies;
global using Unit = System.ValueTuple;
global using static CSharp.Lessons.Functional.Extensions;

using CSharp.Lessons.DbData;
using CSharp.Lessons.BusinessLogic;

// This does not compile. So, you can't bind one generic parameter.
//global using ResultData<T> = CSharp.Lessons.Functional.Result<T, ErrorData>

Console.WriteLine("Starting...");

var employeeId = new EmployeeId(10L);

EmployeeEmail createEmail() =>
    EmployeeEmail.TryCreate(Guid.NewGuid().ToString("N") + EmployeeEmail.CorporateDomain)
    .DefaultWith(e => throw new InvalidDataException($"{e}"));

EmployeeName createName() =>
    EmployeeName.TryCreate(Guid.NewGuid().ToString("N"))
    .DefaultWith(e => throw new InvalidDataException($"{e}"));

Console.WriteLine("Creating EmployeeProxy...");
var proxy = ConnectionString.DefaultValue.CreateEmployeeProxy();
Console.WriteLine("EmployeeProxy created.\n\n");

Console.WriteLine($"Trying to load employee with employeeId: {employeeId}.");
var employeeResult = proxy.LoadEmployee(employeeId);
Console.WriteLine($"Result: '{employeeResult}'.\n\n");
Console.ReadLine();

Console.WriteLine("Trying to create a new employee...");

var newEmployee = new Employee(
    EmployeeId: EmployeeId.DefaultValue,
    EmployeeName: createName(),
    EmployeeEmail: createEmail(),
    ManagedBy: None,
    DateHired: DateTime.Now,
    Salary: 100,
    Description: None,
    Data: new EmployeeData[]
        {
            new EmployeeData(EmployeeDataType.FavoriteRestaurant, None),
            new EmployeeData(EmployeeDataType.PetName, Some("Max")),
        }
        .ToImmutableDictionary(e => e.EmployeeDataType));

Console.WriteLine("New employee created.\n\n");
Console.WriteLine("Trying to save a new employee...");
var result = proxy.SaveEmployee(newEmployee);
Console.WriteLine($"Result: '{result}'.\n\n");
Console.ReadLine();

var salaryRaise = SalaryRaiseByPct.TryCreate(0.2m)
    .Map(e => (SalaryRaise)e);

var (employees, failed) = proxy.LoadAll().UnzipResults();
Console.WriteLine($"Loaded: {employees.Count()} employees, failed: {failed.Count()}");

foreach (var f in failed)
{
    Console.WriteLine($"Failure: {f}.");
}

foreach (var e in employees)
{
    Console.WriteLine($"Name: {e.EmployeeName.Value}, salary: {e.Salary}.");
}

var (newEmpl, newFailures) = salaryRaise
    .Map(e => e.RaiseAll(employees))
    .MapList()
    .UnzipResults();

Console.ReadLine();

Console.WriteLine($"Raised salary for: {newEmpl.Count()} employees, failed: {newFailures.Count()}");

foreach (var e in newEmpl)
{
    Console.WriteLine($"Name: {e.EmployeeName.Value}, salary: {e.Salary}.");
    var r1 = proxy.SaveEmployee(e);
    Console.WriteLine($"Saved: {r1.IsOk}.");
}

Console.ReadLine();
