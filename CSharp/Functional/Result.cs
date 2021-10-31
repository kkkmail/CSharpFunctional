// Based on https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Either.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public static partial class Extensions
{
    public static Result.Ok<TResult> Ok<TResult>(TResult result) => new Result.Ok<TResult>(result);
    public static Result.Error<TError> Error<TError>(TError error) => new Result.Error<TError>(error);
    public static Unit Unit { get; } = new Unit();
    public static Result.Ok<Unit> Ok() => new Result.Ok<Unit>(Unit);

    public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g) =>
        a => g(f(a));
}

public record struct Result<TResult, TError>
{
    internal TResult Ok { get; }
    internal TError Error { get; }

    public bool IsOk { get; }
    public bool IsError => !IsOk;

    internal Result(TResult result)
    {
        IsOk = true;
        Ok = result;
        Error = default(TError)!;
    }

    internal Result(TError error)
    {
        IsOk = false;
        Ok = default(TResult)!;
        Error = error;
    }

    public static implicit operator Result<TResult, TError>(TResult result) => new Result<TResult, TError>(result);
    public static implicit operator Result<TResult, TError>(TError error) => new Result<TResult, TError>(error);

    public static implicit operator Result<TResult, TError>(Result.Ok<TResult> result) => new Result<TResult, TError>(result.Value);
    public static implicit operator Result<TResult, TError>(Result.Error<TError> error) => new Result<TResult, TError>(error.Value);

    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => IsOk ? ok(Ok) : error(Error);

    public IEnumerator<TError> AsErrorEnumerable()
    {
        if (IsError) yield return Error;
    }

    public IEnumerator<TResult> AsResultEnumerable()
    {
        if (IsOk) yield return Ok;
    }

    public override string ToString() => Match(r => $"Ok({r})", e => $"Error({e})");
}

public static class Result
{
    public record struct Ok<TResult>
    {
        internal TResult Value { get; }
        internal Ok(TResult value) { Value = value; }
        public override string ToString() => $"Ok({Value})";
        public Ok<TNewResult> Map<TNewResult, TError>(Func<TResult, TNewResult> f) => Ok(f(Value));
        public Result<TNewResult, TError> Bind<TNewResult, TError>(Func<TResult, Result<TNewResult, TError>> f) => f(Value);
    }

    public record struct Error<TError>
    {
        internal TError Value { get; }
        internal Error(TError value) { Value = value; }
        public override string ToString() => $"Error({Value})";
        public Error<TNewError> Map<TResult, TNewError>(Func<TError, TNewError> f) => Error(f(Value));
        public Result<TResult, TNewError> Bind<TResult, TNewError>(Func<TError, Result<TResult, TNewError>> f) => f(Value);
    }
}

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
