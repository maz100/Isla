using System;
using System.Linq;
using System.Reflection;
using Moq;

namespace Isla.Testing.Moq
{
	public class MockInjectionException : Exception
	{
		public MockInjectionException( string message )
			: base( message )
		{
		}
	}
}