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

    public interface ISomeClass
    {

    }

    public class SomeClass
    {
        private ISomeDependency1 _someDependency1;
        private ISomeDependency2 _someDependency2;

        public SomeClass(ISomeDependency1 someDependency1, ISomeDependency2 someDependency2)
        {
            _someDependency1 = someDependency1;
            _someDependency2 = someDependency2;
        }

        public void SomeMethod()
        {
            _someDependency1.SomeMethod1();
            _someDependency2.SomeMethod2();
        }
    }

    public interface ISomeDependency1
    {
        void SomeMethod1();
    }

    public interface ISomeDependency2
    {
        void SomeMethod2();
    }
}
