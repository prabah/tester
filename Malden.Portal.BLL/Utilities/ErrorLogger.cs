using System;
using System.Diagnostics;
using System.Text;

namespace Malden.Portal.BLL.Utilities
{
    public static class ErrorLogger
    {
        public static void Log(Exception exception)
        {
            LogException(exception);

            var innerException = exception.InnerException;

            while (innerException != null)
            {
                LogException(innerException);

                innerException = innerException.InnerException;
            }
        }

        private static void LogException(Exception exception)
        {
            var sbExceptionMessage = new StringBuilder();

            sbExceptionMessage.Append("Type: " + Environment.NewLine);
            sbExceptionMessage.Append(exception.GetType().Name);
            sbExceptionMessage.Append(Environment.NewLine + Environment.NewLine);
            sbExceptionMessage.Append("Message: " + Environment.NewLine);
            sbExceptionMessage.Append(exception.Message + Environment.NewLine + Environment.NewLine);
            sbExceptionMessage.Append("Trace: " + Environment.NewLine);
            sbExceptionMessage.Append(exception.StackTrace + Environment.NewLine + Environment.NewLine);

            const string source = "MaldenPortal";

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, source);
            }

            var eventLog = new EventLog("MaldenPortal") { Source = source };

            eventLog.WriteEntry(sbExceptionMessage.ToString(), EventLogEntryType.Error);

            eventLog.Dispose();
        }
    }
}