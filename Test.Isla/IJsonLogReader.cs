using System.Collections.Generic;
using Isla.Logging;


namespace Test.Isla
{
	public interface IJsonLogReader
	{
		IEnumerable<LogMessage> GetLogMessages (string path);
	}
}

