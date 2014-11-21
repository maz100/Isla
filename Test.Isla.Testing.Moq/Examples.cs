using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isla.Testing.Moq;
using log4net;
using Moq;
using NUnit.Framework;

namespace Test.Isla.Testing.Moq
{
    [TestFixture]
    public class Examples
    {
        public void CreateAnAutomockedInsance()
        {
            
            //create an automocked instance of SomeClassCtor
            var someClass = MoqAutoMocker.CreateInstance<SomeClassCtor>();

            //create an automocked instance of SomeClassProp
            var someClass2 = MoqAutoMocker.CreateInstance<SomeClassProp>();

            //access mocks like this
            someClass.Mock<ISomeDependency1>().Setup(x => x.SomeMethod1());

            //and verify like this
            someClass.VerifyAll();

            //need to create a standalone mock which is not a dependency?Easy:
            someClass.Create<ILog>().Setup(x => x.Debug("message"));

            //above is equivalent to:
            var mocks = new MockRepository(MockBehavior.Default);
            mocks.Create<ILog>().Setup(x => x.Debug("message"));

            //so instead of
            someClass.VerifyAll();
            mocks.VerifyAll();

            //you just need this:
            someClass.VerifyAll();
        }

        public void CreateAutomockedInstanceWithLogging()
        {
            //create automocked class as a proxy with logging interceptors applied to it and its dependencies
            var someClass = MoqAutoMocker.Configure()
                                         .EnableLogging()
                                         .ProxyInstance<ISomeClass, SomeClassCtor>();

            //so this call:
            someClass.SomeMethod("hello world");


        }
    }
}
