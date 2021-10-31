namespace CSharp.Lessons.Functional;

public static class Extensions
{
    #region Result

    public static Result.Ok<TResult> Ok<TResult>(TResult result) => new Result.Ok<TResult>(result);
    public static Result.Error<TError> Error<TError>(TError error) => new Result.Error<TError>(error);
    public static Unit Unit { get; } = new Unit();
    public static Result.Ok<Unit> Ok() => new Result.Ok<Unit>(Unit);

    #endregion

    #region Option

    public static Option<T> Some<T>(T value) => new Option.Some<T>(value); // wrap the given value into a Some
    public static Option.None None => Option.None.Default;  // the None value

    #endregion

    #region Composition

    public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g) =>
        a => g(f(a));

    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    #endregion

    #region Set Elements

    public static Func<TValue, Result<TValue, TError>> NoValidation<TValue, TError>() => v => Ok(v);

    private static Func<TValue, Result<TValue, TError>> CanNotBeNull<TValue, TError>(Func<TValue, TError> errorCreator) =>
        v => v.CanNotBeNull() ? Ok(v) : errorCreator(v);

    public static Result<TSetElement, TError> TryCreate<TSetElement, TValue, TError>(
        TValue value,
        Func<TValue, TSetElement> creator,
        Func<TValue, TError> errorCreator,
        Func<TValue, Result<TValue, TError>>? extraValidator = null) =>
        CanNotBeNull(errorCreator).Compose(r => r.Bind(extraValidator ?? NoValidation<TValue, TError>()))(value).Map(creator);

    #endregion
}
