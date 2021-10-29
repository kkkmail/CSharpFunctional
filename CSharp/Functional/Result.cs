// Based on https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Either.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public static partial class F
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

    //public Unit Match(Action<TResult> Ok, Action<TError> Error)
    //   => Match(Ok.ToFunc(), Error.ToFunc());

    public IEnumerator<TError> AsEnumerable()
    {
        if (IsError) yield return Error;
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
    public static (ImmutableList<TResult> Successes, ImmutableList<TError> Failures) UnzipResults<TResult, TError>(
        this IEnumerable<Result<TResult, TError>> resultList)
    {
        var (s, f) = resultList.Partition(e => e.IsOk);

        var successes = s
            .Select(e => e.Ok)
            .ToImmutableList();

        var failures = f
            .Select(e => e.Error)
            .ToImmutableList();

        return (successes, failures);
    }

    public static Result<TNewResult, TError> Map<TResult, TNewResult, TError>(
        this Result<TResult, TError> t,
        Func<TResult, TNewResult> ok) =>
        t.Match<Result<TNewResult, TError>>(
            r => Ok(ok(r)),
            e => Error(e));

    public static Result<TResult, TNewError> MapError<TResult, TError, TNewError>(
        this Result<TResult, TError> t,
        Func<TError, TNewError> error) =>
        t.Match<Result<TResult, TNewError>>(
            r => Ok(r),
            e => Error(error(e)));

    //public static Result<TNewResult, TNewError> Map<TResult, TNewResult, TError, TNewError>(
    //    this Result<TResult, TError> t,
    //    Func<TResult, TNewResult> ok,
    //    Func<TError, TNewError> error) =>
    //    t.Match<Result<TNewResult, TNewError>>(
    //        r => Ok(ok(r)),
    //        e => Error(error(e)));

    //public static Result<TResult, Unit> ForEach<TResult, TError>
    //   (this Result<TResult, TError> @this, Action<TError> act)
    //   => Map(@this, act.ToFunc());

    public static Result<TNewResult, TError> Bind<TResult, TNewResult, TError>(
        this Result<TResult, TError> t,
        Func<TResult, Result<TNewResult, TError>> ok) =>
        t.Match(
            r => ok(r),
            e => Error(e));

    public static Result<TResult, TNewError> BindError<TResult, TError, TNewError>(
        this Result<TResult, TError> t,
        Func<TError, Result<TResult, TNewError>> error) =>
        t.Match(
            r => Ok(r),
            e => error(e));

    public static Result<TNewResult, TError> Apply<TResult, TNewResult, TError>(
        this Result<Func<TResult, TNewResult>, TError> t,
        Result<TResult, TError> arg) =>
        t.Match(
            ok: success =>
                arg.Match<Result<TNewResult, TError>>(
                    ok: r => Ok(success(r)),
                    error: e => Error(e)),
            error: e => Error(e));

    //public static Result<TResult, TNewError> Apply<TResult, TError, TNewError>(
    //    this Result<TResult, Func<TError, TNewError>> t,
    //    Result<TResult, TError> arg) =>
    //    t.Match(
    //        ok: r => Ok(r),
    //        error: f =>
    //        arg.Match<Result<TResult, TNewError>>(
    //            ok: r1 => Ok(r1),
    //            error: e => Error(f(e))));

    //public static Result<TResult, Func<T2, TError>> Apply<TResult, T1, T2, TError>
    //   (this Result<TResult, Func<T1, T2, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.Curry), arg);

    //public static Result<TResult, Func<T2, T3, TError>> Apply<TResult, T1, T2, T3, TError>
    //   (this Result<TResult, Func<T1, T2, T3, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, TError>> Apply<TResult, T1, T2, T3, T4, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, T5, TError>> Apply<TResult, T1, T2, T3, T4, T5, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, T5, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, T5, T6, TError>> Apply<TResult, T1, T2, T3, T4, T5, T6, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, T5, T6, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, T5, T6, T7, TError>> Apply<TResult, T1, T2, T3, T4, T5, T6, T7, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, T5, T6, T7, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, T5, T6, T7, T8, TError>> Apply<TResult, T1, T2, T3, T4, T5, T6, T7, T8, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, T5, T6, T7, T8, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //public static Result<TResult, Func<T2, T3, T4, T5, T6, T7, T8, T9, TError>> Apply<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, TError>
    //   (this Result<TResult, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TError>> @this, Result<TResult, T1> arg)
    //   => Apply(@this.Map(F.CurryFirst), arg);

    //// LINQ

    //public static Result<TResult, TError> Select<TResult, T, TError>(
    //    this Result<TResult, T> t,
    //    Func<T, TError> map) =>
    //    t.MapError(map);

    //public static Result<TResult, TNewError> SelectMany<TResult, T, TError, TNewError>(
    //    this Result<TResult, T> t,
    //    Func<T, Result<TResult, TError>> bind,
    //    Func<T, TError, TNewError> project) =>
    //    t.Match(
    //        ok: r => Ok(r),
    //        error: e =>
    //            bind(t.Error).Match<Result<TResult, TNewError>>(
    //            ok: l => Ok(l),
    //            error: e1 => project(e, e1)));
}
