namespace CSharp.Lessons.Functional;

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
