// Based on: https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Option.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

public static partial class F
{
    public static Option<T> Some<T>(T value) => new Option.Some<T>(value); // wrap the given value into a Some
    public static Option.None None => Option.None.Default;  // the None value
}

public record struct Option<T>
{
    readonly T value;
    readonly bool isSome;
    bool isNone => !isSome;

    private Option(T value)
    {
        if (value == null) throw new ArgumentNullException();
        this.isSome = true;
        this.value = value;
    }

    public static implicit operator Option<T>(Option.None _) => new Option<T>();
    public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);

    public static implicit operator Option<T>(T value)
        => value == null ? None : Some(value);

    public R Match<R>(Func<R> none, Func<T, R> some)
        => isSome ? some(value) : none();

    public IEnumerable<T> AsEnumerable()
    {
        if (isSome) yield return value;
    }

    public override string ToString() => isSome ? $"Some({value})" : "None";
}

public static class Option
{
    public struct None
    {
        internal static readonly None Default = new None();
    }

    public struct Some<T>
    {
        internal T Value { get; }

        internal Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value)
                    , "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }
}

public static class OptionExt2
{
    /// <summary>
    /// For database interop only!
    /// </summary>
    public static Option<T> ToOption<T>(this T? value)
        where T : struct => value != null ? Some(value.Value) : None;

    /// <summary>
    /// For database interop only!
    /// </summary>
    public static T? FromOption<T>(this Option<T> value)
        where T : struct => value.IsSome() ? value.ValueUnsafe() : null;
}

public static class OptionExt
{
    /// <summary>
    /// For database interop only!
    /// </summary>
    public static Option<T> ToOption<T>(this T? value)
        where T : class => value != null ? Some(value) : None;

    /// <summary>
    /// For database interop only!
    /// </summary>
    public static T? FromOption<T>(this Option<T> value)
        where T : class => value.GetOrElse((T?)null!);

    public static Option<R> Apply<T, R>
        (this Option<Func<T, R>> t, Option<T> arg)
        => t.Match(
            none: () => None,
            some: f => arg.Match(
                none: () => None,
                some: v => Some(f(v))));


    public static Option<R> Bind<T, R>
        (this Option<T> t, Func<T, Option<R>> f)
        => t.Match(
            none: () => None,
            some: v => f(v));

    //public static IEnumerable<R> Bind<T, R>
    //    (this Option<T> t, Func<T, IEnumerable<R>> func)
    //    => t.AsEnumerable().Bind(func);

    //public static Option<Unit> ForEach<T>(this Option<T> t, Action<T> action)
    //    => Map(t, action.ToFunc());

    public static Option<R> Map<T, R>
        (this Option.None _, Func<T, R> f)
        => None;

    public static Option<R> Map<T, R>
        (this Option.Some<T> v, Func<T, R> f)
        => Some(f(v.Value));

    public static Option<R> Map<T, R>
        (this Option<T> optT, Func<T, R> f)
        => optT.Match(
            none: () => None,
            some: v => Some(f(v)));

    //public static Option<Func<T2, R>> Map<T1, T2, R>
    //    (this Option<T1> t, Func<T1, T2, R> func)
    //    => t.Map(func.Curry());

    //public static Option<Func<T2, T3, R>> Map<T1, T2, T3, R>
    //    (this Option<T1> t, Func<T1, T2, T3, R> func)
    //    => t.Map(func.CurryFirst());

    //public static IEnumerable<Option<R>> Traverse<T, R>(this Option<T> t
    //    , Func<T, IEnumerable<R>> func)
    //    => t.Match(
    //        () => List((Option<R>)None),
    //        (t) => func(t).Map(r => Some(r)));

    // utilities

    //public static Unit Match<T>(this Option<T> t, Action None, Action<T> Some)
    //    => t.Match(None.ToFunc(), Some.ToFunc());

    internal static bool IsSome<T>(this Option<T> t)
        => t.Match(
            none: () => false,
            some: _ => true);

    internal static T ValueUnsafe<T>(this Option<T> t)
        => t.Match(
            none: () => { throw new InvalidOperationException(); },
            some: t => t);

    public static T GetOrElse<T>(this Option<T> opt, T defaultValue)
        => opt.Match(
            none: () => defaultValue,
            some: t => t);

    public static T GetOrElse<T>(this Option<T> t, Func<T> fallback)
        => t.Match(
            none: () => fallback(),
            some: t => t);

    //public static Task<T> GetOrElse<T>(this Option<T> opt, Func<Task<T>> fallback)
    //    => opt.Match(
    //        () => fallback(),
    //        (t) => Async(t));

    public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
        => left.Match(
            none: () => right,
            some: _ => left);

    public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
        => left.Match(
            none: () => right(),
            some: _ => left);

    // LINQ

    public static Option<R> Select<T, R>(this Option<T> t, Func<T, R> func)
        => t.Map(func);

    public static Option<T> Where<T>
        (this Option<T> t, Func<T, bool> predicate)
        => t.Match(
            none: () => None,
            some: v => predicate(v) ? t : None);

    public static Option<RR> SelectMany<T, R, RR>
        (this Option<T> t, Func<T, Option<R>> bind, Func<T, R, RR> project)
        => t.Match(
            none: () => None,
            some: v => bind(v).Match(
                none: () => None,
                some: v1 => Some(project(v, v1))));
}
