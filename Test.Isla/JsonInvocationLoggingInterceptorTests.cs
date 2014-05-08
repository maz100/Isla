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

namespace Test.Isla
{
	[TestFixture ()]
	public class JsonInvocationLoggingInterceptorTests
	{
		private JsonInvocationLoggingInterceptor _jsonInvocationLoggingInterceptor;

		[SetUp]
		public void SetUp ()
		{
			_jsonInvocationLoggingInterceptor = MoqAutoMocker.CreateInstance<JsonInvocationLoggingInterceptor> ();
		}

		[Test ()]
		public void TestIntercept ()
		{
			//we expect a timespan and an invocation to be logged via the logging interceptor
			var invocation = new Mock<IInvocation> ();
			invocation.Setup (x => x.InvocationTarget).Returns (new SomeClass ());
			invocation.Setup (x => x.Method).Returns (typeof(SomeClass).GetMethod ("SomeMethod"));
			invocation.Setup (x => x.Arguments).Returns (new object[]{ "hello" });
			invocation.Setup (x => x.ReturnValue).Returns ("hello");
			invocation.Setup (x => x.Proceed ());

			//use a json serialiser to serialise the timed incovation
			string serialisedTimedInvocation = "some serialised timespan";	
			_jsonInvocationLoggingInterceptor.Get <IJsonSerializer> ()
				.Setup (x => x.Serialize (It.Is<TimedInvocation> (y => y.ElapsedTime != null &&
			y.MethodName == invocation.Object.Method.Name &&
			y.Arguments == invocation.Object.Arguments &&
			y.ReturnValue == invocation.Object.ReturnValue)))
				.Returns (serialisedTimedInvocation);
					
			//then use an ILog to log the serialised object at info level
			var log = new Mock<ILog> ();
			string loggerName = new SomeClass ().ToString ();
			_jsonInvocationLoggingInterceptor.Get<ILogManager> ().Setup (x => x.GetLogger (loggerName)).Returns (log.Object);
			log.Setup (x => x.Info (serialisedTimedInvocation));

			//method under test
			_jsonInvocationLoggingInterceptor.Intercept (invocation.Object);

			invocation.VerifyAll ();
			log.VerifyAll ();
			_jsonInvocationLoggingInterceptor.VerifyAll ();
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
	}
}

