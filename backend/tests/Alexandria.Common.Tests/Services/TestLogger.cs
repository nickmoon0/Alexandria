using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Alexandria.Common.Tests.Services;

public class TestLogger<T> : ILogger<T>
{
    private readonly ConcurrentQueue<string> _logMessages = new();

    public IEnumerable<string> LogMessages => _logMessages;

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string>? formatter)
    {
        if (formatter == null) return;
        var message = formatter(state, exception);
        _logMessages.Enqueue($"{logLevel}: {message}");
    }
}