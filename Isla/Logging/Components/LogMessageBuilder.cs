namespace Isla.Logging.Components
{
    public class LogMessageBuilder : ILogMessageBuilder
    {
        #region ILogMessageBuilder implementation

        public LogMessage Build(RawLogMessage rawLogMessage, TimedInvocation timedInvocation)
        {
            var logMessage = new LogMessage();

            logMessage.Date = rawLogMessage.Date;
            logMessage.Level = rawLogMessage.Level;
            logMessage.Logger = rawLogMessage.Logger;
            logMessage.TimedInvocation = timedInvocation;

            return logMessage;
        }

        #endregion
    }
}