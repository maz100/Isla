using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Isla.Logging;
using Isla.Testing.Moq;
using log4net.Config;
using Moq;
using NUnit.Framework;
using Isla.Serialisation.Components;
using Isla.Logging.Components;

namespace Test.Isla.Testing.Moq
{
    [TestFixture]
    public class MoqAutoMockerTests
    {
        [Test]
        public void Test_can_automock_class_with_no_default_constructor()
        {
            var someClass = MoqAutoMocker.CreateInstance<SomeClass>();
        }

        [Test]
        public void Test_non_default_constructor_injected()
        {
            var someClass = MoqAutoMocker.CreateInstance<SomeClass>();

            someClass.Mock<ISomeDependency1>().Setup(x => x.SomeMethod1());
            someClass.Mock<ISomeDependency2>().Setup(x => x.SomeMethod2());

            someClass.SomeMethod("hello world");

            someClass.VerifyAll();
        }

        [Test]
        public void TestAddInterceptor()
        {
            var autoMocker = MoqAutoMocker.Configure().EnableLogging();

            Assert.That(autoMocker.Interceptors.Count(x => x.GetType() == typeof(JsonInvocationLoggingInterceptor)) == 1);
        }

        [Test]
        public void TestCreateInstance_as_proxy()
        {
            XmlConfigurator.Configure();
            var proxiedInstance = MoqAutoMocker.Configure().EnableLogging().ProxyInstance<ISomeClass, SomeClass>();
            Assert.That(proxiedInstance.GetType().Name == "ISomeClassProxy");
            var result = proxiedInstance.SomeMethod("hello world");
        }

        [Test]
        public void TestCreateInstanceWithInterfaceProxy_applies_interceptors()
        {
            var mockInterceptor = new Mock<IInterceptor>();
            mockInterceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>()));

            var someClassProxy = MoqAutoMocker
                .Configure()
                .AddInterceptor(mockInterceptor.Object)
                .ProxyInstance<ISomeClass, SomeClass>();

            someClassProxy.SomeMethod("hello world");

            mockInterceptor.VerifyAll();
        }

        [Test]
        public void TestProxyInstance_with_logging_enabled_intercepts_mocked_dependencies()
        {
            XmlConfigurator.Configure();
            var mockInterceptor = new Mock<IInterceptor>();
            mockInterceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>())).Callback<IInvocation>(x => x.Proceed());

            var someClassProxy = MoqAutoMocker
                .Configure()
                .AddInterceptor(mockInterceptor.Object)
                .EnableLogging()
                .ProxyInstance<ISomeClass, SomeClass>();

            var result = someClassProxy.SomeMethod("hello world");

            //should be called 3 times, once for automocked instance
            //and once for each of its two dependencies
            mockInterceptor.Verify(x => x.Intercept(It.IsAny<IInvocation>()), Times.Exactly(3));
        }

        [Test]
        public void Test_proxy_dynamically_instantiated_mock()
        {
            var mocks = new MockRepository(MockBehavior.Default);

            var someClassMock = global::Isla.Testing.Moq.MoqAutoMocker.InvocationHelper
                .InvokeGenericMethodWithDynamicTypeArguments(mocks, a => a.Create<object>(),
                                                                         null,
                                                                         typeof(ISomeClass));
            var loggingInterceptor = new JsonInvocationLoggingInterceptor
            {
                JsonSerializer = new JsonSerializer(),
                LogManager = new LogManager()
            };

            var generator = new ProxyGenerator();

            var mockObject = someClassMock.GetType().GetProperties()[0].GetValue(someClassMock, null);

            var mockObjectType = mockObject.GetType();

            var targetInterface = mockObjectType.GetInterfaces()[0];

            var proxiedMockObject = generator.CreateInterfaceProxyWithTarget(targetInterface, mockObject, loggingInterceptor);

        }

        //do properties get injected 
    }
}
