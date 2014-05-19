using System;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;
using Isla.Logging;
using ServiceStack.Text;
using JsonSerializer = Isla.Serialisation.Components.JsonSerializer;

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
		    using (JsConfig.With(timeSpanHandler: TimeSpanHandler.StandardFormat, dateHandler:DateHandler.ISO8601))
		    {
		        _jsonSerializer = new JsonSerializer();
		        TimedInvocation timedInvocation = new TimedInvocation();
		        timedInvocation.ElapsedTime = new TimeSpan(1, 10, 25);
		        //timedInvocation.Invocation = null;

		        var json = _jsonSerializer.Serialize(timedInvocation);

		        var jsonSerialiser = new ServiceStack.Text.JsonSerializer<TimeSpan>();

		        string serialisedTimespan = jsonSerialiser.SerializeToString(new TimeSpan(1, 2, 3));

		        var typeSerialiser = new ServiceStack.Text.TypeSerializer<TimeSpan>();

		        string typeSerialiedTimespan = typeSerialiser.SerializeToString(new TimeSpan(1, 2, 3));

		        string serialize = new JsonSerializer<DateTime>().SerializeToString(DateTime.Now);
		    }
		}
	}



}

