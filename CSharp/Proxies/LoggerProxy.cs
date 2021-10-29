namespace CSharp.Lessons.Proxies;
public record struct LoggerProxy(Func<ErrorData, Unit> LogError);
