using System.Collections.Generic;
using Isla.Logging;


namespace Isla.Logging.Components
{
	public interface IJsonLogReader
	{
		IEnumerable<LogMessage> GetLogMessages (string path);
	}
}

