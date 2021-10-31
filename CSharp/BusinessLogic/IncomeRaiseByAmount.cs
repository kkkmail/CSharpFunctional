namespace CSharp.Lessons.BusinessLogic;

public record struct IncomeRaiseByAmount : IIncomeRaise
{
    public IncomeRaiseType IncomeRaiseType => IncomeRaiseType.RaiseByAmount;
    public Func<Employee, Employee> RaiseSalary => RaiseSalaryImpl;

    private static decimal MinValue { get; } = 0;
    private static decimal MaxValue { get; } = 10_000;
    private decimal Value { get; }
    private Employee RaiseSalaryImpl(Employee e) => e with { Salary = e.Salary + Value };

    private static Func<decimal, Result<decimal, ErrorData>> IncomeRaiseByAmountValidator { get; } =
    v => v >= MinValue && v <= MaxValue
        ? v
        : new ErrorData($"The value: {v} is not in the range from {MinValue} to {MaxValue}.");

    private IncomeRaiseByAmount(decimal value) => Value = value;

    public static Result<IncomeRaiseByAmount, ErrorData> TryCreate(
        decimal amount) =>
        TryCreate<IncomeRaiseByAmount, decimal, ErrorData>(
            amount,
            e => new IncomeRaiseByAmount(e),
            ErrorData.Ignore, // Won't be hit because decimal is a value type.
            IncomeRaiseByAmountValidator);

    public int CompareTo(IIncomeRaise? other) =>
        Comparer<IIncomeRaise>.Default.Compare(this, other);
}
