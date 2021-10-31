namespace CSharp.Lessons.Functional;

public readonly record struct Result<TResult, TError> : IResult<TResult, TError>
{
    private IResult<TResult, TError> Value { get; }
    public bool IsOk => Value.IsOk;
    public bool IsError => Value.IsError;
    public T Match<T>(Func<TResult, T> ok, Func<TError, T> error) => Value.Match(ok, error);
    public IEnumerator<TError> AsErrorEnumerable() => Value.AsErrorEnumerable();
    public IEnumerator<TResult> AsResultEnumerable() => Value.AsResultEnumerable();
    public override string ToString() => Value.ToString()!;
    private Result(IResult<TResult, TError> result) => Value = result;

    public static implicit operator Result<TResult, TError>(TResult result) =>
        new (new Ok<TResult, TError>(result));

    public static implicit operator Result<TResult, TError>(TError error) =>
        new (new Error<TResult, TError>(error));

    public static implicit operator Result<TResult, TError>(Result.Ok<TResult> result) =>
        new (new Ok<TResult, TError>(result.Value));

    public static implicit operator Result<TResult, TError>(Result.Error<TError> error) =>
        new (new Error<TResult, TError>(error.Value));
}

public static class Result
{
    public readonly record struct Ok<TResult>
    {
        internal TResult Value { get; }
        internal Ok(TResult value) => Value = value;
        public override string ToString() => $"Ok({Value})";
        public Ok<TNewResult> Map<TNewResult, TError>(Func<TResult, TNewResult> f) => Ok(f(Value));
        public Result<TNewResult, TError> Bind<TNewResult, TError>(Func<TResult, Result<TNewResult, TError>> f) => f(Value);
    }

    public readonly record struct Error<TError>
    {
        internal TError Value { get; }
        internal Error(TError value) => Value = value;
        public override string ToString() => $"Error({Value})";
        public Error<TNewError> Map<TResult, TNewError>(Func<TError, TNewError> f) => Error(f(Value));
        public Result<TResult, TNewError> Bind<TResult, TNewError>(Func<TError, Result<TResult, TNewError>> f) => f(Value);
    }
}
