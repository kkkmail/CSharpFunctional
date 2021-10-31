namespace CSharp.Lessons.BusinessLogic;

public record struct SalaryRaiseByPct : ISalaryRaise
{
    public SalaryRaiseType SalaryRaiseType => SalaryRaiseType.RaiseByPct;
    public Func<Employee, Employee> RaiseSalary => RaiseSalaryImpl;

    private static decimal MinValue => 0;
    private static decimal MaxValue => 1;
    private decimal Value { get; }
    private Employee RaiseSalaryImpl(Employee e) => e with { Salary = e.Salary * (1 + Value) };

    private static Func<decimal, Result<decimal, ErrorData>> SalaryRaiseByPctValidator { get; } =
        v => v >= MinValue && v <= MaxValue
            ? v
            : new ErrorData($"The value: {v} is not in the range from {MinValue} to {MaxValue}.");

    private SalaryRaiseByPct(decimal value) => Value = value;

    public static Result<SalaryRaise, ErrorData> TryCreate(decimal amount) =>
        TryCreate<SalaryRaise, decimal, ErrorData>(
            amount,
            e => new SalaryRaiseByPct(e),
            ErrorData.Ignore, // Won't be hit because decimal is a value type.
            SalaryRaiseByPctValidator);
}
