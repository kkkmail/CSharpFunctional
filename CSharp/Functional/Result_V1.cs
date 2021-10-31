// Based on https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Either.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

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
