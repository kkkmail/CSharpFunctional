namespace CSharp.Lessons.Functional;

public record struct Result1<TResult, TError>
{

    internal Result1(IResult<TResult, TError> result)
    {

    }
}
