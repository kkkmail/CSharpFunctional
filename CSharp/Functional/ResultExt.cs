// Based on https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Either.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public static class ResultExt
{
    public static ImmutableList<Result<TResult, TError>> ToImmutableList<TResult, TError>(this Result<TResult, TError> t) =>
        new Result<TResult, TError>[]
        {
                    t,
        }
        .ToImmutableList();

    public static (IEnumerable<TResult> Successes, IEnumerable<TError> Failures) UnzipResults<TResult, TError>(
        this IEnumerable<Result<TResult, TError>> resultList)
    {
        var (s, f) = resultList.Partition(e => e.IsOk);
        var successes = s.Select(e => e.Ok);
        var failures = f.Select(e => e.Error);
        return (successes, failures);
    }

    public static TResult DefaultValue<TResult, TError>(this Result<TResult, TError> result, TResult defaultValue) =>
        result.Match(
            ok: r => r,
            error: e => defaultValue);

    public static TResult DefaultWith<TResult, TError>(this Result<TResult, TError> result, Func<TResult> getDefaultValue) =>
        result.Match(
            ok: r => r,
            error: e => getDefaultValue());

    /// <summary>
    /// Use to throw exception with error information.
    /// </summary>
    public static TResult DefaultWith<TResult, TError>(this Result<TResult, TError> result, Func<TError, TResult> getDefaultValue) =>
        result.Match(
            ok: r => r,
            error: e => getDefaultValue(e));

    public static Result<TNewResult, TError> Map<TResult, TNewResult, TError>(
        this Result<TResult, TError> result,
        Func<TResult, TNewResult> ok) =>
        result.Match<Result<TNewResult, TError>>(
            ok: r => Ok(ok(r)),
            error: e => Error(e));

    public static Result<TResult, TNewError> MapError<TResult, TError, TNewError>(
        this Result<TResult, TError> result,
        Func<TError, TNewError> error) =>
        result.Match<Result<TResult, TNewError>>(
            ok: r => Ok(r),
            error: e => Error(error(e)));

    public static Result<TResult, TError> MapOption<TResult, TError>(
        this Result<Option<TResult>, TError> result,
        Func<TError> error) =>
        result.Match<Result<TResult, TError>>(
            ok: r =>
                r.Match<Result<TResult, TError>>(
                    none: () => Error(error()),
                    some: v => Ok(v)),
            error: e => Error(e));

    public static IEnumerable<Result<TResult, TError>> MapList<TResult, TError>(
        this Result<IEnumerable<TResult>, TError> result) =>
        result.Match<IEnumerable<Result<TResult, TError>>>(
            ok: r => r.Select(e => (Result<TResult, TError>)Ok(e)),
            error: e => new Result<TResult, TError>[] { Error(e) });

    public static Result<TNewResult, TError> Bind<TResult, TNewResult, TError>(
        this Result<TResult, TError> result,
        Func<TResult, Result<TNewResult, TError>> ok) =>
        result.Match(
            ok: r => ok(r),
            error: e => Error(e));

    public static Result<TResult, TNewError> BindError<TResult, TError, TNewError>(
        this Result<TResult, TError> result,
        Func<TError, Result<TResult, TNewError>> error) =>
        result.Match(
            ok: r => Ok(r),
            error: e => error(e));

    public static Result<TNewResult, TError> Apply<TResult, TNewResult, TError>(
        this Result<Func<TResult, TNewResult>, TError> result,
        Result<TResult, TError> arg) =>
        result.Match(
            ok: success =>
                arg.Match<Result<TNewResult, TError>>(
                    ok: r => Ok(success(r)),
                    error: e => Error(e)),
            error: e => Error(e));
}
