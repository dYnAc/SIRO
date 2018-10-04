
namespace SIRO.Core.Helpers
{
    using System;
    using EPiServer.Logging;

    /// <summary>
    /// The log helper.
    /// </summary>
    internal static class LogHelper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = LogManager.GetLogger();

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Log(string message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void Log(string message, Exception e)
        {
            Logger.Debug(message, e);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void Log(Exception e)
        {
            Logger.Debug(e.Message, e);
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        public static void Information(string message, Type from)
        {
            Logger.Information(message, from);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        public static void Error(string message, object from)
        {
            Logger.Error(message, from);
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        public static void Warning(string message, object from)
        {
            Logger.Warning(message, from);
        }
    }
}
