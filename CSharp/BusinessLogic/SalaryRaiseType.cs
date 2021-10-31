namespace CSharp.Lessons.BusinessLogic;

public record SalaryRaiseType : ClosedSetBase<SalaryRaiseType, long, ErrorData>
{
    public string Name { get; }

    private SalaryRaiseType(long value, [CallerMemberName] string name = null!) : base(value) =>
        Name = name;

    public static SalaryRaiseType RaiseByPct { get; } = new SalaryRaiseType(1);
    public static SalaryRaiseType RaiseByAmount { get; } = new SalaryRaiseType(2);
}
