using NUnit.Framework;
using System;
using Isla.Testing.Moq;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using FizzWare.NBuilder;
using Isla.Logging;

namespace Test.Isla.Logging.Components
{
	[TestFixture ()]
	public class LogMessageFactoryTests
	{
		private LogMessageFactory _logMessageFactory;

		[SetUp]
		public void SetUp ()
		{
			_logMessageFactory = MoqAutoMocker.CreateInstance<LogMessageFactory> ();
		}

		[Test ()]
		public void TestCreate ()
		{
			string logMessageText = "test log message";

			RawLogMessage rawLogMessage = Builder<RawLogMessage>.CreateNew ().Build ();

			TimedInvocation timedInvocation = Builder<TimedInvocation>.CreateNew ().Build ();

			LogMessage expectedlogMessage = Builder<LogMessage>.CreateNew ().Build ();

			var s = _logMessageFactory.Mock<IJsonSerializer> ();

			s.Setup (x => x.Deserialize<RawLogMessage> (logMessageText)).Returns (rawLogMessage);

			s.Setup (x => x.Deserialize<TimedInvocation> (rawLogMessage.Message)).Returns (timedInvocation);

			_logMessageFactory.Mock<ILogMessageBuilder> ().Setup (x => x.Build (rawLogMessage, timedInvocation)).Returns (expectedlogMessage);

			var actualLogMessage = _logMessageFactory.Create (logMessageText);

			Assert.AreEqual (expectedlogMessage, actualLogMessage);
		}
	}
}

