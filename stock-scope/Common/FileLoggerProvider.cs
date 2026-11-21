using System.Text;
using Microsoft.Extensions.Logging;

namespace StockScope.Common
{
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logDirectory;
        private readonly Lock _lock = new();

        public FileLoggerProvider(string logDirectory)
        {
            _logDirectory = logDirectory;
            Directory.CreateDirectory(_logDirectory);
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _logDirectory, _lock);

        public void Dispose()
        {
        }

        private sealed class FileLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly string _logDirectory;
            private readonly Lock _lock;

            public FileLogger(string categoryName, string logDirectory, Lock @lock)
            {
                _categoryName = categoryName;
                _logDirectory = logDirectory;
                _lock = @lock;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

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

                var message = formatter(state, exception);
                if (string.IsNullOrEmpty(message) && exception == null)
                {
                    return;
                }

                // Keep entries single-line to prevent log injection
                var sanitizedMessage = message
                    .Replace("\r", " ", StringComparison.Ordinal)
                    .Replace("\n", " ", StringComparison.Ordinal);

                var now = DateTime.Now;
                var line = new StringBuilder()
                    .Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                    .Append(" [")
                    .Append(GetShortLevel(logLevel))
                    .Append("] ")
                    .Append(_categoryName)
                    .Append(": ")
                    .Append(sanitizedMessage);

                if (exception != null)
                {
                    var sanitizedEx = exception.ToString()
                        .Replace("\r", " ", StringComparison.Ordinal)
                        .Replace("\n", " ", StringComparison.Ordinal);
                    line.Append(" | Exception: ").Append(sanitizedEx);
                }

                var filePath = Path.Combine(_logDirectory, $"stockscope-{now:yyyy-MM-dd}.log");

                lock (_lock)
                {
                    File.AppendAllText(filePath, line.AppendLine().ToString(), Encoding.UTF8);
                }
            }

            private static string GetShortLevel(LogLevel logLevel) => logLevel switch
            {
                LogLevel.Trace => "TRC",
                LogLevel.Debug => "DBG",
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "CRT",
                _ => "UNK"
            };
        }
    }
}

