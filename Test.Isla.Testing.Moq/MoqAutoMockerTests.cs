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

            someClass.SomeMethod();

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
            var result = MoqAutoMocker.Configure().EnableLogging().ProxyInstance<ISomeClass, SomeClass>();
            Assert.That(result.GetType().Name == "ISomeClassProxy");
            result.SomeMethod();
        }

        [Test]
        public void TestCreateInstanceWithInterfaceProxy_applies_interceptors()
        {
            var mockInterceptor = new Mock<IInterceptor>();
            mockInterceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>()));

            var someClassProxy = MoqAutoMocker
                .Configure()
                .AddInterceptor(mockInterceptor.Object)
                .ProxyInstance<ISomeClass,SomeClass>();

            someClassProxy.SomeMethod();

            mockInterceptor.VerifyAll();
        }

        //do properties get injected
    }
}
