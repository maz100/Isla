using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isla.Logging;
using Isla.Testing.Moq;
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
            var autoMocker = MoqAutoMocker.EnableLogging();

            Assert.That(autoMocker.Interceptors.Count(x => x.GetType() == typeof(JsonInvocationLoggingInterceptor)) == 1);
        }

        //do properties get injected
    }
}
