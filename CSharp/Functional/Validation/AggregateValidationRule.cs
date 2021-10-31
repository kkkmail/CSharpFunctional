namespace CSharp.Lessons.Functional.Validation;

public abstract record AggregateValidationRule<TValue, TError> : IValidationRule<TValue, TError>
    where TValue : IComparable<TValue>
{
    private ImmutableList<IValidationRule<TValue, TError>> NonAggregatableRules { get; }
    private ImmutableList<IValidationRule<TValue, TError>> AggregatableRules { get; }
    private Func<TError, TError, TError> CombineErrors { get; }
    public bool CanAggregate { get; }

    public AggregateValidationRule(
        IEnumerable<IValidationRule<TValue, TError>> rules,
        Func<TError, TError, TError> combineErrors,
        bool failOnFirst = false)
    {
        CanAggregate = !failOnFirst;

        if (failOnFirst)
        {
            AggregatableRules = ImmutableList<IValidationRule<TValue, TError>>.Empty;
            NonAggregatableRules = rules.ToImmutableList();
        }
        else
        {
            var (t, f) = rules.Partition(e => e.CanAggregate);
            AggregatableRules = t.ToImmutableList();
            NonAggregatableRules = f.ToImmutableList();
        }

        CombineErrors = combineErrors;
    }

    public Result<TValue, TError> Validate(TValue value)
    {
        foreach (var rule in NonAggregatableRules)
        {
            var result = rule.Validate(value);
            if (result.IsError) return result;
        }

        var failedRules = AggregatableRules
            .Select(e => e.Validate(value))
            .Where(e => e.IsError)
            .Select(e => e.Error)
            .ToList();

        if (failedRules.Count > 0)
        {
            var result = failedRules.Aggregate((e, a) => CombineErrors(e, a));
            return result;
        }

        return Ok(value);
    }
}
