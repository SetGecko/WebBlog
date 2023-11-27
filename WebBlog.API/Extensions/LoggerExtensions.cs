namespace WebBlog.Extensions
{
    /// <summary>
    /// Класс содержит заранее созданные делегаты для Logger
    /// такой подход обеспечивает лучшую производительность при логировании
     /// </summary>
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception?> 
            commonError = LoggerMessage.Define<string>(
                logLevel: LogLevel.Error,
                eventId: 1,
                formatString: "Executing action at '{StartTime}'");

        private static readonly Action<ILogger, string, Exception?>
            commonInfo = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information,
                eventId: 2,
                formatString: "Executing action at '{StartTime}'");

        private static readonly Action<ILogger, string, Exception?>
           commonWarn = LoggerMessage.Define<string>(
               logLevel: LogLevel.Warning,
               eventId: 3,
               formatString: "Executing action at '{StartTime}'");

        private static readonly Action<ILogger, string, Exception?>
           recordUserAction = LoggerMessage.Define<string>(
               logLevel: LogLevel.Information,
               eventId: 4,
               formatString: "Executing action at '{StartTime}'");

        /// <summary>
        /// Записывает строку msg и объект ex в лог уровня LogLevel.Error и ID равным 1.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void CommonError(
            this ILogger logger,Exception? ex ,string msg)
        {
            commonError(logger, msg, ex);
        }
        /// <summary>
        /// Записывает строку msg  в лог уровня LogLevel.Information и ID равным 2.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void CommonInfo(
            this ILogger logger, string msg)
        {
            commonInfo(logger, msg, null);
        }
        /// <summary>
        /// Записывает строку msg  в лог уровня LogLevel.Warning и ID равным 3.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void CommonWarn(
            this ILogger logger, string msg)
        {
            commonWarn(logger, msg, null);
        }

        /// <summary>
        /// Записывает строку msg  в лог уровня LogLevel.Information и ID равным 4.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void RecordUserAction(
            this ILogger logger, string msg)
        {
            recordUserAction(logger, msg, null);
        }
    }
}
