using System;

namespace Isla.Logging.Components
{
	public interface ILogMessageBuilder
	{
		LogMessage Build (RawLogMessage rawLogMessage, TimedInvocation timedInvocation);
	}

	public class LogMessageBuilder : ILogMessageBuilder
	{
		#region ILogMessageBuilder implementation
		public LogMessage Build (RawLogMessage rawLogMessage, TimedInvocation timedInvocation)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

