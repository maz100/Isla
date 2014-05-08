using System;
using Isla.Serialisation.Components;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;

namespace Test.Isla.Serialisation.Components
{

	public class SomeClass :ISomeClass
	{
		#region ISomeClass implementation

		public string SomeMethod (string input)
		{
			return input;
		}

		#endregion
	}
}
