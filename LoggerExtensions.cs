using System.Runtime.CompilerServices;
using MediaBrowser.Model.Logging;

namespace Emby.Plugins.Douban
{
    public static class LoggerExtensions
    {
        public static void LogCallerInfo(this ILogger logger, string message, [CallerMemberName] string caller = null)
        {
            Log(logger, LogSeverity.Info, caller, message);
        }

        public static void LogCallerWarning(this ILogger logger, string message, [CallerMemberName] string caller = null)
        {
            Log(logger, LogSeverity.Warn, caller, message);
        }

        public static void LogCallerError(this ILogger logger, string message, [CallerMemberName] string caller = null)
        {
            Log(logger, LogSeverity.Error, caller, message);
        }

        private static void Log(ILogger logger, LogSeverity logSeverity, string caller, string message)
        {
            logger.Log(logSeverity, $"[{caller}] {message}");
        }
    }
}