using System;
using Castle.DynamicProxy;
using System.Diagnostics;
using log4net;
using Isla.Serialisation.Components;
using Isla.Logging.Components;
using ServiceStack.Text;

namespace Isla.Logging
{
	public class JsonInvocationLoggingInterceptor : IInterceptor
	{
		public IJsonSerializer JsonSerializer { get; set; }

		public ILogManager LogManager { get; set; }

		#region IInterceptor implementation

		public void Intercept (IInvocation invocation)
		{
			var logger = LogManager.GetLogger (invocation.InvocationTarget.ToString ());

			var stopwatch = Stopwatch.StartNew ();

			invocation.Proceed ();

			stopwatch.Stop ();

			var timedInvocation = new TimedInvocation (invocation);
			timedInvocation.ElapsedTime = stopwatch.Elapsed;

			var jsonTimedInvocation = JsonSerializer.Serialize (timedInvocation);

			logger.Info (jsonTimedInvocation);
		}

		#endregion
	}
}

