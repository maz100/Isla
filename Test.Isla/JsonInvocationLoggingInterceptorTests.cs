using System;
using System.IO;
using System.Linq;
using Castle.DynamicProxy;
using Castle.Windsor;
using Isla;
using Isla.Logging;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using Isla.Testing.Moq;
using log4net;
using log4net.Config;
using log4net.Layout;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using ServiceStack.Text;
using Test.Isla.Serialisation.Components;

namespace Test.Isla
{
    [TestFixture]
    public class JsonInvocationLoggingInterceptorTests
    {
        private WindsorContainer _container;
        private JsonInvocationLoggingInterceptor _interceptor;

        [SetUp]
        public void SetUp()
        {
            _container = new WindsorContainer();

            _container.Install(new IslaInstaller(), new TestInstaller());

            _interceptor = MoqAutoMocker.CreateInstance<JsonInvocationLoggingInterceptor>();
        }

        [Test]
        public void TestIntercept()
        {
            var invocation = SetupInvocationMock();

            //use a json serialiser to serialise the timed incovation
            var serialisedTimedInvocation = "some serialised timespan";
            _interceptor.Mock<IJsonSerializer>()
                .Setup(x => x.Serialize(It.Is<TimedInvocation>(y => matchTimedInvocation(y, invocation.Object))))
                .Returns(serialisedTimedInvocation);

            //then use an ILog to log the serialised object at info level
            var log = _interceptor.Create<ILog>();
            var loggerName = new SomeClass().GetType().Name.ToString();
            _interceptor.Mock<ILogManager>().Setup(x => x.GetLogger(loggerName)).Returns(log.Object);
            log.Setup(x => x.Info(serialisedTimedInvocation));

            //method under test
            _interceptor.Intercept(invocation.Object);

            _interceptor.VerifyAll();
        }

        [Test]
        public void TestIntercept_write_begin_and_end_debug_log_statements()
        {
            var logMock = _interceptor.Create<ILog>();
            var beginMessage = "some begin message";
            var endMessage = "some end message";

            var invocation = SetupInvocationMock();

            _interceptor.Mock<ILogManager>().Setup(x => x.GetLogger(It.IsAny<string>())).Returns(logMock.Object);

            //we expect the serialiser to return a begin message based on the invocation
            //the timed invocation should have an empty string value for elapsed time
            _interceptor.Mock<IJsonSerializer>().Setup(x => x.Serialize(It.Is<BeginTimedInvocation>(y => (matchTimedInvocation(y.Begin, invocation.Object))))).Returns(beginMessage);

            //the begin message should be logged at debug level
            logMock.Setup(x => x.Debug(beginMessage));
            TimeSpan result;

            //here we expect the serialiser to return an end message based on the invocation
            //this time the elapsed time value should be set - this is a job of the intercept
            //method which is being tested
            _interceptor.Mock<IJsonSerializer>().Setup(x => x.Serialize(It.Is<EndTimedInvocation>(y => (matchTimedInvocation(y.End, invocation.Object))))).Returns(endMessage);

            //the end Message should also be logged
            logMock.Setup(x => x.Debug(endMessage));

            //method under test
            _interceptor.Intercept(invocation.Object);

            _interceptor.VerifyAll();
        }

        [Test]
        public void TestIntercept_HandlesErrorAndPreservesStackTrace()
        {
            var invocation = SetupInvocationMock();

            invocation.Setup(x => x.Proceed()).Throws(createException());

            //use a json serialiser to serialise the timed incovation
            var serialisedTimedInvocation = "some serialised timespan";
            _interceptor.Mock<IJsonSerializer>()
                .Setup(x => x.Serialize(It.Is<TimedInvocation>(y => matchTimedInvocation(y, invocation.Object))))
                .Returns(serialisedTimedInvocation);

            //then use an ILog to log the serialised object at info level
            var log = _interceptor.Create<ILog>();
            var loggerName = new SomeClass().GetType().Name.ToString();
            _interceptor.Mock<ILogManager>().Setup(x => x.GetLogger(loggerName)).Returns(log.Object);
            log.Setup(x => x.Error(serialisedTimedInvocation));

            //method under test
            try
            {
                _interceptor.Intercept(invocation.Object);
            }
            catch (ApplicationException ex)
            {
                _interceptor.VerifyAll();
                return;
            }

            Assert.Fail(); // If no exception was caught, something went wrong!
        }

        private Exception createException()
        {
            return new ApplicationException();
        }

        [Test]
        public void TestUsingInstaller()
        {
            var someClass = _container.Resolve<ISomeClass>();

            for (var i = 0; i < 10; i++)
            {
                someClass.SomeMethod("hello world");
            }
        }

        [Test]
        public void TestUsingInstaller_error()
        {
            var someClass = _container.Resolve<ISomeClass>();

            var expectedErrorThrown = false;

            try
            {
                someClass.SomeMethod("");
            }
            catch (ArgumentNullException ex)
            {
                expectedErrorThrown = true;
            }

            Assert.That(expectedErrorThrown);
        }

        [Test]
        public void TestSerialize()
        {
            var timedInvocation = new TimedInvocation();
            timedInvocation.MethodName = "test method name";
            timedInvocation.Arguments = new object[] { "hello", 3, DateTime.Now };
            timedInvocation.ReturnValue = "test return value";
            timedInvocation.ElapsedTime = new TimeSpan(1, 2, 3);
            var serialiser = new JsonStringSerializer();
            var serialisedInvocation = serialiser.SerializeToString(timedInvocation);

            var deserialisedInstance = serialiser.DeserializeFromString<TimedInvocation>(serialisedInvocation);

            Assert.AreEqual(timedInvocation.MethodName, deserialisedInstance.MethodName);
        }

        private Mock<IInvocation> SetupInvocationMock()
        {
            var invocation = _interceptor.Create<IInvocation>();
            invocation.Setup(x => x.InvocationTarget).Returns(new SomeClass());
            invocation.Setup(x => x.Method).Returns(typeof(SomeClass).GetMethod("SomeMethod"));
            invocation.Setup(x => x.Arguments).Returns(new object[] { "hello" });
            invocation.Setup(x => x.ReturnValue).Returns("hello");

            invocation.Setup(x => x.Proceed());

            return invocation;
        }

        private bool matchTimedInvocation(TimedInvocation y, IInvocation invocation)
        {
            return y.ElapsedTime != null &&
                   y.MethodName == invocation.Method.Name &&
                   y.Arguments == invocation.Arguments &&
                   y.ReturnValue == invocation.ReturnValue;
        }

        [Test]
        public void TestMixins()
        {
            var mockRepositoryProvider = new MockRepositoryProvider();
            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(mockRepositoryProvider);

            var x =
                (JsonInvocationLoggingInterceptor)
                    generator.CreateClassProxy(typeof(JsonInvocationLoggingInterceptor), null, options);

            var loggerName = "test logger";

            var expectedLog = x.Mocks().Of<ILog>().First();

            var logManager = x.Mocks().Of<ILogManager>().First(y => y.GetLogger(loggerName) == expectedLog);

            var actualLog = logManager.GetLogger(loggerName);

            Assert.AreEqual(expectedLog, actualLog);

            x.Mocks().VerifyAll();
        }

        [Test]
        public void Test_create_mock_with_interceptor()
        {
            XmlConfigurator.Configure();
            var someClassMock = new Mock<ISomeClass>();
            var loggingInterceptor = MoqAutoMocker.CreateInstance<JsonInvocationLoggingInterceptor>();
            var logMock = loggingInterceptor.Create<ILog>();

            var serialisedInvocation = "some test invocation";

            loggingInterceptor.Mock<IJsonSerializer>()
                .Setup(x => x.Serialize(It.IsAny<TimedInvocation>()))
                .Returns(serialisedInvocation);

            logMock.Setup(x => x.Info(serialisedInvocation));

            loggingInterceptor.Mock<ILogManager>().Setup(x => x.GetLogger(It.IsAny<string>())).Returns(logMock.Object);

            var generator = new ProxyGenerator();
            var someClass = generator.CreateInterfaceProxyWithTarget(someClassMock.Object, loggingInterceptor);

            someClass.SomeMethod("hello world");

            loggingInterceptor.VerifyAll();
        }

        [Test]
        public void TestJsonLayout()
        {
            var serializedLayout = new SerializedLayout();
        }

        [Test]
        public void TestDeserialiseLogEntry()
        {
            var jsonLogEntry =
                @"{""date"":""2014-05-15T15:39:54.6832140+01:00"",""level"":""INFO"",""sitename"":""test-domain-Test.Isla.dll"",""logger"":""Test.Isla.Serialisation.Components.SomeClass"",""thread"":""TestRunnerThread"",""message"":""{\""MethodName\"":\""SomeMethod\"",\""Arguments\"":[\""hello world\""],\""ReturnValue\"":\""hello world\"",\""ElapsedTime\"":\""00:00:00.0003261\""}""}";

            var s = new JsonStringSerializer();

            var result = s.DeserializeFromString<RawLogMessage>(jsonLogEntry);

            var message = result.Message;

            var inv = s.DeserializeFromString<TimedInvocation>(message);
        }

        [Test, Category("Example")]
        public void TestReadFromFile()
        {
            var lines = File.ReadAllLines("log.txt");

            var s = new JsonStringSerializer();

            var rawLogMessages = lines.Select(s.DeserializeFromString<RawLogMessage>);

            var logMessages = rawLogMessages.Where(x => x.Level == "INFO").Select(x => new LogMessage
            {
                Date = x.Date,
                Level = x.Level,
                Logger = x.Logger,
                TimedInvocation = s.DeserializeFromString<TimedInvocation>(x.Message)
            }).ToList();

            var searchResults = logMessages.Where(x => x.TimedInvocation.Arguments[0].Equals("hello world")).ToList();

            //var longestRunningCall = logMessages.First (x => x.TimedInvocation.ElapsedTime == logMessages.Max (y => y.TimedInvocation.ElapsedTime));

            //var errorsInLastHour = logMessages.Count (x => x.Date > DateTime.Now.AddHours (-1) && x.Level == "ERROR");
        }

        [Test]
        public void TestSerializeUsingJsonNet()
        {
            var timedInvocation = new TimedInvocation();
            timedInvocation.MethodName = "test method name";
            timedInvocation.Arguments = new object[] { "hello", 3, DateTime.Now };
            timedInvocation.ReturnValue = "test return value";
            timedInvocation.ElapsedTime = new TimeSpan(1, 2, 3);

            var serialisedInvocation = JsonConvert.SerializeObject(timedInvocation);

            var deserialisedInstance = JsonConvert.DeserializeObject<TimedInvocation>(serialisedInvocation);

            Assert.AreEqual(timedInvocation.MethodName, deserialisedInstance.MethodName);
        }

        [Test, Category("Example")]
        public void TestDeserializeLogFile()
        {
            var logReader = _container.Resolve<IJsonLogReader>();

            var logMessages = logReader.GetLogMessages("log.txt");

            var errors = logMessages.Where(x => x.Level == "ERROR")
                .ToList();
        }
    }
}