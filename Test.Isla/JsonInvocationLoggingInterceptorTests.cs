using NUnit.Framework;
using System;
using Isla;
using Castle.Core;
using Moq;
using Castle.DynamicProxy;
using Isla.Testing.Moq;
using System.Diagnostics;
using System.Reactive.Concurrency;
using log4net;
using Isla.Serialisation.Components;
using Isla.Logging.Components;
using Isla.Logging;
using Castle.Windsor;
using Test.Isla.Serialisation.Components;
using log4net.Config;
using Castle.MicroKernel.Registration;
using ServiceStack.Text;
using Ninject.MockingKernel.Moq;
using System.Collections.Generic;
using System.Linq;

namespace Test.Isla
{
	[TestFixture ()]
	public class JsonInvocationLoggingInterceptorTests
	{
		private JsonInvocationLoggingInterceptor _interceptor;

		[SetUp]
		public void SetUp ()
		{
			_interceptor = MoqAutoMocker.CreateInstance<JsonInvocationLoggingInterceptor> ();
		}

		[Test ()]
		public void TestIntercept ()
		{
			var invocation = SetupInvocationMock ();

			//use a json serialiser to serialise the timed incovation
			string serialisedTimedInvocation = "some serialised timespan";	
			_interceptor.Mock <IJsonSerializer> ()
				.Setup (x => x.Serialize (It.Is<TimedInvocation> (y => matchTimedInvocation (y, invocation.Object))))
				.Returns (serialisedTimedInvocation);
					
			//then use an ILog to log the serialised object at info level
			var log = _interceptor.Create<ILog> ();
			string loggerName = new SomeClass ().ToString ();
			_interceptor.Mock<ILogManager> ().Setup (x => x.GetLogger (loggerName)).Returns (log.Object);
			log.Setup (x => x.Info (serialisedTimedInvocation));

			//method under test
			_interceptor.Intercept (invocation.Object);

			_interceptor.VerifyAll ();
		}

		[Test]
		public void TestUsingInstaller ()
		{
			XmlConfigurator.Configure ();

			var container = new WindsorContainer ();


			container.Install (new IslaInstaller ());

			container.Register (Classes.FromThisAssembly ()
				.Where (x => x.Namespace.Contains ("Components"))
				.WithServiceFirstInterface ()
				.Configure (x => x.Interceptors<JsonInvocationLoggingInterceptor> ()));
		
			var someClass = container.Resolve<ISomeClass> ();

			someClass.SomeMethod ("hello world");
		}

		[Test]
		public void TestSerialize ()
		{
			var timedInvocation = new TimedInvocation ();
			timedInvocation.MethodName = "test method name";
			timedInvocation.Arguments = new object[]{ "hello", 3, DateTime.Now };
			timedInvocation.ReturnValue = "test return value";
			timedInvocation.ElapsedTime = new TimeSpan (1, 2, 3);
			var serialiser = new JsonStringSerializer ();
			var serialisedInvocation = serialiser.SerializeToString (timedInvocation);

			var deserialisedInstance = serialiser.DeserializeFromString<TimedInvocation> (serialisedInvocation);
		}

		private Mock<IInvocation> SetupInvocationMock ()
		{
			var invocation = _interceptor.Create<IInvocation> ();
			invocation.Setup (x => x.InvocationTarget).Returns (new SomeClass ());
			invocation.Setup (x => x.Method).Returns (typeof(SomeClass).GetMethod ("SomeMethod"));
			invocation.Setup (x => x.Arguments).Returns (new object[]{ "hello" });
			invocation.Setup (x => x.ReturnValue).Returns ("hello");

			invocation.Setup (x => x.Proceed ());

			return invocation;
		}

		private bool matchTimedInvocation (TimedInvocation y, IInvocation invocation)
		{
			return y.ElapsedTime != null &&
			y.MethodName == invocation.Method.Name &&
			y.Arguments == invocation.Arguments &&
			y.ReturnValue == invocation.ReturnValue;
		}

		[Test]
		public void TestMixins ()
		{
			var mockRepositoryProvider = new MockRepositoryProvider ();
			var generator = new ProxyGenerator ();
			var options = new ProxyGenerationOptions ();
			options.AddMixinInstance (mockRepositoryProvider);

			var x = (JsonInvocationLoggingInterceptor)generator.CreateClassProxy (typeof(JsonInvocationLoggingInterceptor), null, options);

			var loggerName = "test logger";

			var expectedLog = x.Mocks ().Of<ILog> ().First ();

			var logManager = x.Mocks ().Of<ILogManager> ()
				.Where (y => y.GetLogger (loggerName) == expectedLog)
				.First ();

			var actualLog = logManager.GetLogger (loggerName);

			Assert.AreEqual (expectedLog, actualLog);

			x.Mocks ().VerifyAll ();

		}
	}
}

