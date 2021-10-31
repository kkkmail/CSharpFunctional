// Based on: https://github.com/la-yumba/functional-csharp-code/blob/master/LaYumba.Functional/Option.cs
// But with some tweaks.

namespace CSharp.Lessons.Functional;

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
        where T : class => value.AsEnumerable().FirstOrDefault();

    /// <summary>
    /// For database interop only!
    /// </summary>
    public static TValue? FromOption<TSetElement, TValue>(this Option<TSetElement> option)
        where TSetElement : SetBase<TSetElement, TValue>
        where TValue : class, IComparable<TValue> =>
        option.Map(x => x.Value).FromOption();

    public static Option<R> Apply<T, R>(this Option<Func<T, R>> t, Option<T> arg) =>
        t.Match(
            none: () => None,
            some: f => arg.Match(
                none: () => None,
                some: v => Some(f(v))));

    public static Option<R> Bind<T, R>(this Option<T> t, Func<T, Option<R>> f) =>
        t.Match(
            none: () => None,
            some: v => f(v));

    public static Option<R> Map<T, R>(this Option.None _, Func<T, R> f) => None;

    public static Option<R> Map<T, R>(this Option.Some<T> v, Func<T, R> f) =>
        Some(f(v.Value));

    public static Option<R> Map<T, R>(this Option<T> optT, Func<T, R> f) =>
        optT.Match(
            none: () => None,
            some: v => Some(f(v)));

    internal static bool IsSome<T>(this Option<T> t) =>
        t.Match(
            none: () => false,
            some: _ => true);

    internal static T ValueUnsafe<T>(this Option<T> t) =>
        t.Match(
            none: () => throw new InvalidOperationException(),
            some: t => t);

    public static T GetOrElse<T>(this Option<T> opt, T defaultValue) =>
        opt.Match(
            none: () => defaultValue,
            some: t => t);

    public static T GetOrElse<T>(this Option<T> t, Func<T> fallback) =>
        t.Match(
            none: fallback,
            some: t => t);

    public static Option<T> OrElse<T>(this Option<T> left, Option<T> right) =>
        left.Match(
            none: () => right,
            some: _ => left);

    public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right) =>
        left.Match(
            none: right,
            some: _ => left);

    // LINQ - TODO kk:20211030 - I am not sure that this is very valuable.

    public static Option<R> Select<T, R>(this Option<T> t, Func<T, R> func) => t.Map(func);

    public static Option<T> Where<T>(this Option<T> t, Func<T, bool> predicate) =>
        t.Match(
            none: () => None,
            some: v => predicate(v) ? t : None);

    public static Option<RR> SelectMany<T, R, RR>(this Option<T> t, Func<T, Option<R>> bind, Func<T, R, RR> project) =>
        t.Match(
            none: () => None,
            some: v => bind(v).Match(
                none: () => None,
                some: v1 => Some(project(v, v1))));
}
