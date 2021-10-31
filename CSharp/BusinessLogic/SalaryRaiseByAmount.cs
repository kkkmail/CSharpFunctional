namespace CSharp.Lessons.BusinessLogic;

public record struct SalaryRaiseByAmount : ISalaryRaise
{
    public SalaryRaiseType SalaryRaiseType => SalaryRaiseType.RaiseByAmount;
    public Func<Employee, Employee> RaiseSalary => RaiseSalaryImpl;

    private static decimal MinValue { get; } = 0;
    private static decimal MaxValue { get; } = 10_000;
    private decimal Value { get; }
    private Employee RaiseSalaryImpl(Employee e) => e with { Salary = e.Salary + Value };

    private static Func<decimal, Result<decimal, ErrorData>> SalaryRaiseByAmountValidator { get; } =
        v => v >= MinValue && v <= MaxValue
            ? v
            : new ErrorData($"The value: {v} is not in the range from {MinValue} to {MaxValue}.");

    private SalaryRaiseByAmount(decimal value) => Value = value;

    public static Result<SalaryRaiseByAmount, ErrorData> TryCreate(decimal amount) =>
        TryCreate<SalaryRaiseByAmount, decimal, ErrorData>(
            amount,
            e => new SalaryRaiseByAmount(e),
            ErrorData.Ignore, // Won't be hit because decimal is a value type.
            SalaryRaiseByAmountValidator);

    public int CompareTo(ISalaryRaise? other) =>
        Comparer<ISalaryRaise>.Default.Compare(this, other);
}
