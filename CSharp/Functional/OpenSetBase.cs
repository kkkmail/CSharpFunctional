namespace CSharp.Lessons.Functional;

public abstract record OpenSetBase<TSetElement, TValue>
    : SetBase<TSetElement, TValue>
    where TSetElement : OpenSetBase<TSetElement, TValue>
    where TValue : IComparable<TValue>
{
    protected OpenSetBase(TValue value) : base(value)
    {
    }
}
