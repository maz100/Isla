using System;
using Isla.Logging;
using NUnit.Framework;
using ServiceStack.Text;
using JsonSerializer = Isla.Serialisation.Components.JsonSerializer;

namespace Test.Isla.Serialisation.Components
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private JsonSerializer _jsonSerializer;

        [Test]
        public void TestSerialise()
        {
            using (JsConfig.With(timeSpanHandler: TimeSpanHandler.StandardFormat, dateHandler: DateHandler.ISO8601))
            {
                _jsonSerializer = new JsonSerializer();
                var timedInvocation = new TimedInvocation();
                timedInvocation.ElapsedTime = new TimeSpan(1, 10, 25);
                //timedInvocation.Invocation = null;

                var json = _jsonSerializer.Serialize(timedInvocation);

                var jsonSerialiser = new JsonSerializer<TimeSpan>();

                var serialisedTimespan = jsonSerialiser.SerializeToString(new TimeSpan(1, 2, 3));

                var typeSerialiser = new TypeSerializer<TimeSpan>();

                var typeSerialiedTimespan = typeSerialiser.SerializeToString(new TimeSpan(1, 2, 3));

                var serialize = new JsonSerializer<DateTime>().SerializeToString(DateTime.Now);
            }
        }
    }
}