using System;
using Isla.Serialisation.Components;
using Isla;
using Moq;
using Castle.DynamicProxy;
using NUnit;
using NUnit.Framework;

namespace Test.Isla.Serialisation.Components
{

	public class DummyInvocation : IInvocation
	{
		#region IInvocation implementation

		public object GetArgumentValue (int index)
		{
			throw new NotImplementedException ();
		}

		public System.Reflection.MethodInfo GetConcreteMethod ()
		{
			throw new NotImplementedException ();
		}

		public System.Reflection.MethodInfo GetConcreteMethodInvocationTarget ()
		{
			throw new NotImplementedException ();
		}

		public void Proceed ()
		{
			throw new NotImplementedException ();
		}

		public void SetArgumentValue (int index, object value)
		{
			throw new NotImplementedException ();
		}

		public object[] Arguments {
			get {
				return new []{ "this is a test argument" };
			}
		}

		public Type[] GenericArguments {
			get {
				return null;
			}
		}

		public object InvocationTarget {
			get {
				throw new NotImplementedException ();
			}
		}

		public System.Reflection.MethodInfo Method {
			get {
				throw new NotImplementedException ();
			}
		}

		public System.Reflection.MethodInfo MethodInvocationTarget {
			get {
				throw new NotImplementedException ();
			}
		}

		public object Proxy {
			get {
				throw new NotImplementedException ();
			}
		}

		public object ReturnValue {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Type TargetType {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
	
}
