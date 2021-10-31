namespace CSharp.Lessons.Functional;

public interface IOption<T>
{
    R Match<R>(Func<R> none, Func<T, R> some);
    IEnumerable<T> AsEnumerable();
}
