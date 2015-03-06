using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Isla.Logging;
using Moq;
using NUnit.Framework;
using Test.Isla.Serialisation.Components;

namespace Test.Isla.Logging
{
    [TestFixture]
    public class TimedInvocationTests
    {
        private Mock<IInvocation> _invocationMock;
        private IInvocation _invocation;

        [SetUp]
        public void SetUp()
        {
            _invocationMock = new Mock<IInvocation>();
            _invocationMock.Setup(x => x.InvocationTarget).Returns(new SomeClass());
            _invocationMock.Setup(x => x.Method).Returns(typeof(SomeClass).GetMethod("SomeMethod"));
            _invocationMock.Setup(x => x.Arguments).Returns(new object[] { "hello" });
            _invocationMock.Setup(x => x.ReturnValue).Returns("hello");

            _invocationMock.Setup(x => x.Proceed());

            _invocation = _invocationMock.Object;
        }

        [Test]
        [TestCase(InvocationSerialisation.Arguments, false)]
        [TestCase(InvocationSerialisation.Both, false)]
        [TestCase(InvocationSerialisation.None, true)]
        [TestCase(InvocationSerialisation.ReturnValue, true)]
        public void TestArgumentSerialisationIsSuppressed(InvocationSerialisation invocationSerialisation, bool suppressSerialisation)
        {
            var expected = _invocation.Arguments.Select(argument => argument.GetType().ToString()).ToList();

            var timedInvocation = new TimedInvocation(_invocation, invocationSerialisation);

            var actual = timedInvocation.Arguments;

            Assert.That(actual.SequenceEqual(expected) == suppressSerialisation);
        }

        [Test]
        [TestCase(InvocationSerialisation.Arguments, true)]
        [TestCase(InvocationSerialisation.Both, false)]
        [TestCase(InvocationSerialisation.None, true)]
        [TestCase(InvocationSerialisation.ReturnValue, false)]
        public void TestReturnValueSerialisationIsSuppressed(InvocationSerialisation invocationSerialisation, bool shouldSuppressSerialisation)
        {
            var suppressedValue = _invocation.ReturnValue.GetType().ToString();

            var timedInvocation = new TimedInvocation(_invocation, invocationSerialisation);

            var isSerialistionSuppressed = (suppressedValue.Equals(timedInvocation.ReturnValue));

            Assert.That(isSerialistionSuppressed == shouldSuppressSerialisation);
        }
    }
}