using System;

namespace Isla.Testing.Moq
{
    public class MockInjectionException : Exception
    {
        public MockInjectionException(string message)
            : base(message)
        {
        }
    }
}