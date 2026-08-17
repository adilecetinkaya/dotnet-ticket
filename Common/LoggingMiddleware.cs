using System.Diagnostics;
using Wolverine;

public static class LoggingMiddleware
{
    public static Stopwatch Before(ILogger<LoggingMarker> logger, Envelope envelope)
    {
        logger.LogInformation("-> {MessageType} basladi", envelope.Message?.GetType().Name);
        return Stopwatch.StartNew();
    }

    public static void Finally(Stopwatch stopwatch, ILogger<LoggingMarker> logger, Envelope envelope)
    {
        stopwatch.Stop();
        logger.LogInformation(
            "<- {MessageType} bitti ({Ms} ms)",
            envelope.Message?.GetType().Name,
            stopwatch.ElapsedMilliseconds);
    }
}

public class LoggingMarker
{
}
