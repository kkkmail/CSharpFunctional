namespace CSharp.Lessons.BusinessLogic;

public record struct IncomeRaiseByPct : IIncomeRaise
{
    public IncomeRaiseType IncomeRaiseType => IncomeRaiseType.RaiseByPct;
    public Func<Employee, Employee> RaiseSalary => RaiseSalaryImpl;

    private static decimal MinValue { get; } = 0;
    private static decimal MaxValue { get; } = 1;
    private decimal Value { get; }
    private Employee RaiseSalaryImpl(Employee e) => e with { Salary = e.Salary * (1 + Value) };

    private static Func<decimal, Result<decimal, ErrorData>> IncomeRaiseByPctValidator { get; } =
    v => v >= MinValue && v <= MaxValue
        ? v
        : new ErrorData($"The value: {v} is not in the range from {MinValue} to {MaxValue}.");

    private IncomeRaiseByPct(decimal value) => Value = value;

    public static Result<IncomeRaiseByPct, ErrorData> TryCreate(
        decimal amount) =>
        TryCreate<IncomeRaiseByPct, decimal, ErrorData>(
            amount,
            e => new IncomeRaiseByPct(e),
            ErrorData.Ignore, // Won't be hit because decimal is a value type.
            IncomeRaiseByPctValidator);
}
