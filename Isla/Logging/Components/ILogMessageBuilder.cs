namespace Isla.Logging.Components
{
    public interface ILogMessageBuilder
    {
        LogMessage Build(RawLogMessage rawLogMessage, TimedInvocation timedInvocation);
    }
}