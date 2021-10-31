namespace CSharp.Lessons.Functional;

internal interface IResult<TResult, TError>
{
    bool IsOk { get; }
    T Match<T>(Func<TResult, T> ok, Func<TError, T> error);
}
