using System;
using System.Linq;
using System.Reflection;
using Moq;

namespace Isla.Testing.Moq
{
	public static class AutoMockExtensions
	{
		public static Mock<T> Mock<T> (this object instance) where T : class
		{
			var propertyInfo = instance.GetType ().GetProperties ().FirstOrDefault (x => x.PropertyType == typeof(T));

			if (propertyInfo == null) {
				throw new MockInjectionException ("Could not locate mock of requested type: " + typeof(T));
			}

			//get the mocked interface
			var value = propertyInfo.GetValue (instance, null);

			//get its Mock property
			var property = value.GetType ().GetProperties ().FirstOrDefault (x => x.Name == "Mock");

			if (property == null) {
				throw new MockInjectionException ("Could not locate mock of requested type: " + typeof(T));
			}

			return property.GetValue (value, null) as Mock<T>;
		}
        
		public static void VerifyAll (this object instance)
		{
            instance.Mocks().VerifyAll();
		}

		//creates a new mock object which will be included in the call to VerifyAll
		public static Mock<T> Create<T> (this object instance) where T : class
		{
			var mocksProvider = instance as IMockRepositoryProvider;
			return mocksProvider.Mocks ().Create<T> ();
		}

		public static MockRepository Mocks (this object instance)
		{
			var mocksProvider = instance as IMockRepositoryProvider;
			return mocksProvider.Mocks ();
		}
	}
}
