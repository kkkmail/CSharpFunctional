namespace CSharp.Lessons.Functional;

public abstract record OpenSetBase<TSetElement, TValue, TError>
    : SetBase<TSetElement, TValue, TError>
    where TSetElement : OpenSetBase<TSetElement, TValue, TError>
    where TValue : IComparable<TValue>
{
    protected OpenSetBase(TValue value) : base(value)
    {
    }
}
