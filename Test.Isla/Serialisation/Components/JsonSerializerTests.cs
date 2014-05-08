using System;
using Isla.Serialisation.Components;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;
using Isla.Logging;

namespace Test.Isla.Serialisation.Components
{
	[TestFixture]
	public class JsonSerializerTests
	{
		private JsonSerializer _jsonSerializer;

		public JsonSerializerTests ()
		{
		}

		[Test]
		public void TestSerialise ()
		{
			_jsonSerializer = new JsonSerializer ();
			TimedInvocation timedInvocation = new TimedInvocation ();
			timedInvocation.ElapsedTime = new TimeSpan (1, 10, 25);
			//timedInvocation.Invocation = null;

			var json = _jsonSerializer.Serialize (timedInvocation);
		}
	}



}

