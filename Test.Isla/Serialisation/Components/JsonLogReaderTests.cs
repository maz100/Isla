using System;
using NUnit.Framework;
using System.Linq;
using Isla.Testing.Moq;
using Isla.Serialisation.Components;
using Isla.Logging;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Isla.Logging.Components;
using Isla.Components;

namespace Test.Isla.Serialisation.Components
{
	[TestFixture]
	public class JsonLogReaderTests
	{
		public JsonLogReaderTests ()
		{
		}

		[Test]
		public void TestRead ()
		{
			var jsonLogReader = MoqAutoMocker.CreateInstance<JsonLogReader> ();

			string path = "log.txt";
			var size = 10;
			var logEventStrings = new string[10];
			var expectedLogMessages = Builder<LogMessage>.CreateListOfSize (size).Build ();

			jsonLogReader.Mock<IFileHelper> ().Setup (x => x.ReadAllLines (path)).Returns (logEventStrings);

			for (int i = 0; i < size; i++) {
				logEventStrings [i] = "test message " + i;
				jsonLogReader.Mock<ILogMessageFactory> ().Setup (x => x.Create (logEventStrings [i])).Returns (expectedLogMessages [i]);
			}

			var actualLogMessages = jsonLogReader.GetLogMessages (path);

			Assert.That (actualLogMessages.SequenceEqual (expectedLogMessages));

			jsonLogReader.VerifyAll ();
		}
	}
}

