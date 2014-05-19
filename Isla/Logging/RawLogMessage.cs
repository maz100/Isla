using System;

namespace Isla.Logging
{
	public class RawLogMessage
	{
		public DateTime Date{ get; set; }

		public string Level{ get; set; }

		public string Logger{ get; set; }

		public string Message{ get; set; }
	}

}

