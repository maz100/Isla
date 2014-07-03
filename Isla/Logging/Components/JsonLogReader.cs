using Isla.Logging.Components;
using System.Collections.Generic;
using Isla.Logging;
using System.Linq;
using Isla.Components;

namespace Isla.Logging.Components
{
	public class JsonLogReader : IJsonLogReader
	{
		public IFileHelper FileHelper { get; set; }

		public ILogMessageFactory LogMessageFactory { get; set; }

		public IEnumerable<LogMessage> GetLogMessages (string path)
		{
			var lines = FileHelper.ReadAllLines (path);

			//default sort by date
			var logMessages = lines.Select (x => LogMessageFactory.Create (x))
				.OrderBy(y=>y.Date)
				.ToList();

			return logMessages;
		}
	}
}

