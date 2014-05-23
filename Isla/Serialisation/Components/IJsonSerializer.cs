using System;
using Castle.DynamicProxy;
using Isla.Logging;

namespace Isla.Serialisation.Components
{
	public interface IJsonSerializer
	{
		string Serialize (TimedInvocation invocation);

		T Deserialize<T> (string source);
	}
}

