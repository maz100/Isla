using System;
using System.Diagnostics;
using Castle.DynamicProxy;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using log4net.Config;
using L4nLogManager = log4net.LogManager;

namespace Isla.Logging
{
	public class JsonInvocationLoggingInterceptor : IInterceptor
	{
		public IJsonSerializer JsonSerializer { get; set; }
		public ILogManager LogManager { get; set; }

		#region IInterceptor implementation

		public void Intercept(IInvocation invocation)
		{
			if (!L4nLogManager.GetRepository().Configured)
			{
				XmlConfigurator.Configure();
			}

			var logger = LogManager.GetLogger(invocation.InvocationTarget.GetType().Name);

			if (log4net.ThreadContext.Properties["callid"] == null)
			{
				log4net.ThreadContext.Properties["callid"] = Guid.NewGuid();
			}

			Exception exception = null;
			Stopwatch stopwatch = null;
			TimedInvocation timedInvocation = null;
			try
			{
				timedInvocation = new TimedInvocation(invocation);
				var beginTimedInvocation = new BeginTimedInvocation(timedInvocation);
				var jsonBeginTimedInvocation = JsonSerializer.Serialize(beginTimedInvocation);

				logger.Debug(jsonBeginTimedInvocation);

				stopwatch = Stopwatch.StartNew();

				invocation.Proceed();
			}
			catch (Exception ex)
			{
				exception = ex;
				throw; // Preserve the stack trace of the exception
			}
			finally
			{
				stopwatch.Stop();

				timedInvocation.ElapsedTime = stopwatch.Elapsed;
				timedInvocation.ReturnValue = invocation.ReturnValue;

				var endTimedInvocation = new EndTimedInvocation(timedInvocation);

				var jsonTimedInvocation = JsonSerializer.Serialize(timedInvocation);
				var jsonEndTimedInvocation = JsonSerializer.Serialize(endTimedInvocation);


				if (exception != null)
				{
					timedInvocation.ExceptionInfo = new ExceptionInfo(exception);
				}

				jsonTimedInvocation = JsonSerializer.Serialize(timedInvocation);

				// Log the final result
				if (exception != null)
				{
					logger.Error(jsonTimedInvocation);
				}
				else
				{
					logger.Info(jsonTimedInvocation);
					logger.Debug(jsonEndTimedInvocation);
				}
			}
		}

		#endregion
	}
}