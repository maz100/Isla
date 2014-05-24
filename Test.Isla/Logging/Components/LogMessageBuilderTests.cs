using NUnit.Framework;
using System;
using Isla.Logging.Components;
using FizzWare.NBuilder;
using Isla.Logging;

namespace Test.Isla.Logging.Components
{
	[TestFixture ()]
	public class LogMessageBuilderTests
	{
		[Test ()]
		public void TestBuild ()
		{
			var logMessageBuilder = new LogMessageBuilder ();

			var rawLogMessage = Builder<RawLogMessage>.CreateNew ().Build ();

			var timedInvocation = Builder<TimedInvocation>.CreateNew ().Build ();

			var logMessage = logMessageBuilder.Build (rawLogMessage, timedInvocation);

			Assert.That (logMessage.Date == rawLogMessage.Date);
			Assert.That (logMessage.Level == rawLogMessage.Level);
			Assert.That (logMessage.Logger == rawLogMessage.Logger);
			Assert.That (logMessage.TimedInvocation == timedInvocation);
		}
	}
}

