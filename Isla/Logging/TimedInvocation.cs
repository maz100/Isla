using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace Isla.Logging
{
	public class TimedInvocation
	{
		public TimedInvocation ()
		{

		}

		public TimedInvocation (IInvocation invocation)
		{
			MethodName = invocation.Method.Name;
			Arguments = invocation.Arguments;
			ReturnValue = invocation.ReturnValue;
		}

		public string MethodName { get; set; }

		public object[] Arguments { get; set; }

		public object ReturnValue { get; set; }

		public Exception Exception  { get; set; }

		public TimeSpan ElapsedTime { get; set; }
	}
}

