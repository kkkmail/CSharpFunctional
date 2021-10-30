namespace CSharp.Lessons.BusinessLogic;

public record IncomeRaiseByPct : OpenSetBase<IncomeRaiseByPct, decimal, ErrorData>, IIncomeRaise
{
    private static decimal MinValue { get; } = 0;
    private static decimal MaxValue { get; } = 1;

    public static Func<decimal, Result<decimal, ErrorData>> IncomeRaiseByAmountValidator { get; } =
    v => v >= MinValue && v <= MaxValue
        ? v
        : new ErrorData($"The value: {v} is not in the range from {MinValue} to {MaxValue}.");

    private IncomeRaiseByPct(decimal value) : base(value)
    {
    }

    public static Result<IncomeRaiseByPct, ErrorData> TryCreate(
        decimal amount) =>
        TryCreate(
            amount,
            e => new IncomeRaiseByPct(e),
            ErrorData.Ignore, // Won't be hit because decimal is a value type.
            IncomeRaiseByAmountValidator);

    public int CompareTo(IIncomeRaise? other) =>
        Comparer<IIncomeRaise>.Default.Compare(this, other);

    public IncomeRaiseType IncomeRaiseType => IncomeRaiseType.RaiseByPct;
    public Func<Employee, Employee> RaiseSalary => e => e with { Salary = e.Salary * (1 + Value) };
}
