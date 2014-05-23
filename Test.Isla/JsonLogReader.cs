using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Isla;
using Isla.Logging;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using Isla.Testing.Moq;
using Moq;
using NUnit.Framework;
using Newtonsoft.Json;
using ServiceStack.Text;
using log4net;
using log4net.Config;
using log4net.Layout;
using Test.Isla.Serialisation.Components;

namespace Test.Isla
{
	public class JsonLogReader : IJsonLogReader
	{
		public IFileHelper FileHelper { get; set; }

		public ILogMessageFactory LogMessageFactory { get; set; }

		public IEnumerable<LogMessage> GetLogMessages (string path)
		{
			var lines = FileHelper.ReadAllLines (path);

			//default sort by date
			var logMessages = lines.Select (x => LogMessageFactory.Create (x)).OrderBy(y=>y.Date);

			return logMessages;
		}
	}
}

