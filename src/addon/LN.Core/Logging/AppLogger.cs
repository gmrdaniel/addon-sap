using System;
using Serilog;
using Serilog.Core;

namespace LN.Core.Logging
{
    /// <summary>
    /// Logger central del add-on. Escribe a archivo local; los eventos de uso
    /// facturables se registran además en la UDT @LN_USOLOG desde los módulos.
    /// </summary>
    public static class AppLogger
    {
        private static Logger? _logger;

        public static void Initialize(string logFilePath)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14)
                .CreateLogger();
        }

        public static void Info(string message) => _logger?.Information(message);

        public static void Warn(string message) => _logger?.Warning(message);

        public static void Error(string message, Exception? ex = null) => _logger?.Error(ex, message);

        public static void Shutdown() => _logger?.Dispose();
    }
}
