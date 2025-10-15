namespace ReactiveETL;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public static class LogProvider
{
    private static readonly Dictionary<string, ILogger> Loggers = new ();
    private static ILoggerFactory loggerFactory = new LoggerFactory();

    public static void SetLogFactory(ILoggerFactory factory)
    {
        loggerFactory?.Dispose();
        loggerFactory = factory;
        Loggers.Clear();
    }

    public static ILogger GetLogger(string category)
    {
        if (!Loggers.TryGetValue(category, out _))
        {
            Loggers[category] = loggerFactory?.CreateLogger(category) ?? NullLogger.Instance;
        }

        return Loggers[category];
    }
}