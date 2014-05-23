using System;

namespace Isla.Logging.Components
{
	public interface ILogMessageFactory
	{
		LogMessage Create (string logEventText);
	}
}

