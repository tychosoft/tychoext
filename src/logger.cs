// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Collections.Concurrent;

namespace Tychosoft.Extensions {
    public static class Logger {
        private class FileLoggerProvider(string path) : ILoggerProvider {
            private readonly string logPath = path;
            private readonly ConcurrentDictionary<string, FileLogger> loggers = new();

            public ILogger CreateLogger(string categoryName) {
                return loggers.GetOrAdd(categoryName, name => new FileLogger(logPath));
            }

            public void Dispose() {
                loggers.Clear();
                GC.SuppressFinalize(this);
            }
        }

        private class FileLogger(string logPath) : ILogger {
            private readonly string path = logPath;
            private static readonly object locker = new();
            private static readonly LogLevel level = LogLevel.Information;

            IDisposable ILogger.BeginScope<TState>(TState state) {
                return BeginScope(state)!;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
            public static IDisposable BeginScope<TState>(TState state) {
                return null!;
            }

            public bool IsEnabled(LogLevel logLevel) => logLevel >= level;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
                if (!IsEnabled(logLevel)) {
                    return;
                }

                ArgumentNullException.ThrowIfNull(formatter);

                var logRecord = $"{logLevel} {DateTime.Now}: {formatter(state, exception)}{Environment.NewLine}";

                lock (locker) {
                    File.AppendAllText(path, logRecord);
                }
            }
        }

        private static ILogger? logger;
        private static ILoggerFactory? factory;
        private static readonly LoggingLevelSwitch level;

        private class LoggingLevelSwitch {
            public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
        }

        static Logger() {
            level = new LoggingLevelSwitch();
        }

        public static void Startup(string name, string? path = null) {
            factory = LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddFilter<ConsoleLoggerProvider>((_, logLevel) => logLevel >= level.MinimumLevel);
                builder.AddSimpleConsole(options => {
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                });

                if (path != null) {
                    builder.AddProvider(new FileLoggerProvider(path));
                }
            });
            logger = factory?.CreateLogger(name);
        }

        public static void Shutdown() {
            if (factory is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        public static void Info(string? message, params object?[] args) {
            logger?.LogInformation("{Message}", [message ?? "null", args]);
        }

        public static void Warn(string? message, params object?[] args) {
            logger?.LogWarning("{Message}", [message ?? "null", args]);
        }

        public static void Error(string? message, params object?[] args) {
            logger?.LogError("{Message}", [message ?? "null", args]);
        }

        public static void Debug(string? message, params object?[] args) {
            logger?.LogDebug("{Message}", [message ?? "null", args]);
        }

        public static void Trace(string? message, params object?[] args) {
            logger?.LogTrace("{Message}", [message ?? "null", args]);
        }

        public static void Fatal(int code, string? message, params object?[] args) {
            logger?.LogCritical("{Message}", [message ?? "null", args]);
            Environment.Exit(code);
        }

        public static void SetQuiet() {
            level.MinimumLevel = LogLevel.Warning;
        }

        public static void SetVerbose() {
            level.MinimumLevel = LogLevel.Trace;
        }
    }
} // end namespace
