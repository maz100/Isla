using System;
using Isla.Serialisation.Components;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;
using ServiceStack.Text;
using Newtonsoft.Json;

namespace Test.Isla.Serialisation.Components
{
	public class SomeClass :ISomeClass
	{
		#region ISomeClass implementation

		public string SomeMethod (string input)
		{
			if (string.IsNullOrEmpty (input)) {
				throw new ArgumentNullException ("input", "parameter cannot be null or empty");
			}
			return input;
		}

		#endregion
	}
}
