namespace CSharp.Lessons.BusinessLogic;

public record IncomeRaiseType : ClosedSetBase<IncomeRaiseType, long, ErrorData>
{
    public string Name { get; }

    private IncomeRaiseType(long value, [CallerMemberName] string name = null!) : base(value) =>
        Name = name;

    public static IncomeRaiseType RaiseByPct { get; } = new IncomeRaiseType(1);
    public static IncomeRaiseType RaiseByAmount { get; } = new IncomeRaiseType(2);
}
