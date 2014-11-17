using Isla.Serialisation.Components;

namespace Isla.Logging.Components
{
    public class LogMessageFactory : ILogMessageFactory
    {
        public IJsonSerializer JsonSerializer { get; set; }
        public ILogMessageBuilder LogMessageBuilder { get; set; }

        #region ILogMessageFactory implementation

        public LogMessage Create(string logEventText)
        {
            var rawLogMessage = JsonSerializer.Deserialize<RawLogMessage>(logEventText);

            var timedInvocation = JsonSerializer.Deserialize<TimedInvocation>(rawLogMessage.Message);

            var logMessage = LogMessageBuilder.Build(rawLogMessage, timedInvocation);

            return logMessage;
        }

        #endregion
    }
}