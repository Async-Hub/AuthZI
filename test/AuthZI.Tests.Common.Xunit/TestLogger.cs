namespace AuthZI.Tests.Common.Xunit;

using global::Xunit.Abstractions;
using Microsoft.Extensions.Logging;

public class TestLogger(ITestOutputHelper output) : ILogger
{
  public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null!;

  public bool IsEnabled(LogLevel logLevel) => true;

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter)
  {
    if (!IsEnabled(logLevel))
    {
      return;
    }

    output.WriteLine(exception != null
      ? $"[{eventId.Id,2}: {logLevel,-12} {exception}]"
      : $"[{eventId.Id,2}: {logLevel,-12}]");
  }
}

public class TestLogger<T>(ITestOutputHelper output) : TestLogger(output), ILogger<T>
{
}