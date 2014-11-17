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

        public void Intercept(IInvocation invocation)
		{
            var logger = LogManager.GetLogger(invocation.InvocationTarget.ToString());

            var stopwatch = Stopwatch.StartNew();

			Exception exception = null;

			try {
                invocation.Proceed();
            }
            catch (Exception ex) {
				exception = ex;
                throw; // Preserve the stack trace of the exception
			}
            finally {
                stopwatch.Stop();

                var timedInvocation = new TimedInvocation(invocation);
			timedInvocation.ElapsedTime = stopwatch.Elapsed;

                if (exception != null)
                {
                    timedInvocation.ExceptionInfo = new ExceptionInfo(exception);
			}

                var jsonTimedInvocation = JsonSerializer.Serialize(timedInvocation);

                // Log the final result
                if (exception != null)
                {
                    logger.Error(jsonTimedInvocation);
                }
                else
                {
                    logger.Info(jsonTimedInvocation);
                }
			}
		}

		#endregion
	}
}

