using System.Linq;
using FizzWare.NBuilder;
using Isla.Components;
using Isla.Logging;
using Isla.Logging.Components;
using Isla.Testing.Moq;
using NUnit.Framework;

namespace Test.Isla.Serialisation.Components
{
    [TestFixture]
    public class JsonLogReaderTests
    {
        [Test]
        public void TestRead()
        {
            var jsonLogReader = MoqAutoMocker.CreateInstance<JsonLogReader>();

            var path = "log.txt";
            var size = 10;
            var logEventStrings = new string[10];
            var expectedLogMessages = Builder<LogMessage>.CreateListOfSize(size).Build();

            jsonLogReader.Mock<IFileHelper>().Setup(x => x.ReadAllLines(path)).Returns(logEventStrings);

            for (var i = 0; i < size; i++)
            {
                logEventStrings[i] = "test message " + i;
                jsonLogReader.Mock<ILogMessageFactory>()
                    .Setup(x => x.Create(logEventStrings[i]))
                    .Returns(expectedLogMessages[i]);
            }

            var actualLogMessages = jsonLogReader.GetLogMessages(path);

            Assert.That(actualLogMessages.SequenceEqual(expectedLogMessages));

            jsonLogReader.VerifyAll();
        }
    }
}