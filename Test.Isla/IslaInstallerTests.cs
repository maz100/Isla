using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Isla;
using Isla.Logging;
using Isla.Serialisation.Components;
using NUnit.Framework;

namespace Test.Isla
{
    [TestFixture]
    public class IslaInstallerTests
    {
        [TestCase(InvocationSerialisation.None)]
        [TestCase(InvocationSerialisation.Both)]
        [TestCase(InvocationSerialisation.Arguments)]
        [TestCase(InvocationSerialisation.ReturnValue)]
        public void TestWith_can_configure_interceptor_serialisation_via_installer(InvocationSerialisation expected)
        {
            var installer = IslaInstaller.With(expected);

            var container = new WindsorContainer();

            container.Install(installer);

            var interceptor = container.Resolve<JsonInvocationLoggingInterceptor>();

            var actual = interceptor.InvocationSerialisation;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestWithIndentation_can_configure_serialisation_indentation_via_installer(bool expected)
        {
            var installer = IslaInstaller.WithIndentation(expected);

            var container = new WindsorContainer();

            container.Install(installer);

            var jsonSerializer = container.Resolve<IJsonSerializer>();

            var actual = jsonSerializer.Indent;

            Assert.AreEqual(expected, actual);
        }
    }
}
