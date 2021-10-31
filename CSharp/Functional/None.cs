namespace CSharp.Lessons.Functional;

public record struct None<T> : IOption<T>
{
    public IEnumerable<T> AsEnumerable()
    {
        yield break;
    }

    public R Match<R>(Func<R> none, Func<T, R> some) => none();
}
