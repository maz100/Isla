using System.Collections.Generic;
using System;
using System.Reflection;
using Moq;
using System.Linq;

namespace Isla.Testing.Moq
{
	public class MoqAutoMocker
	{
		private static IDictionary<Type, object> _mocks;

		public static T CreateInstance<T> () where T : new()
		{
			_mocks = new Dictionary<Type, object> ();

			var propertyInfos = typeof(T).GetProperties ().Where (x => x.PropertyType.IsInterface);

			T instance = new T ();

			foreach (var propertyInfo in propertyInfos) {
				InjectPropertyWithMock (propertyInfo, instance);
			}

			return instance;
		}

		private static void InjectPropertyWithMock (PropertyInfo propertyInfo, object result)
		{
			var myType = propertyInfo.PropertyType;

			var mock = Activator.CreateInstance (typeof(Mock<>).MakeGenericType (myType));

			_mocks [myType] = mock;

			if (!propertyInfo.CanWrite) {
				return;
			}

			propertyInfo.SetValue (result, ((Mock)mock).Object, null);
		}
	}
}


