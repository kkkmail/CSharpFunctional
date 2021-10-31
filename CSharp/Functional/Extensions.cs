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

    #endregion
}
