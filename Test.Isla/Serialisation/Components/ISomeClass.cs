using System;
using Isla.Serialisation.Components;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;

namespace Test.Isla.Serialisation.Components
{

	public interface ISomeClass
	{
		string SomeMethod (string input);
	}
	
}
