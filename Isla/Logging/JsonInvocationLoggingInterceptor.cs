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

            var logger = LogManager.GetLogger(invocation.InvocationTarget.GetType().Name.ToString());

            if (log4net.ThreadContext.Properties["callid"] == null)
            {
                log4net.ThreadContext.Properties["callid"] = Guid.NewGuid();
            }

            var timedInvocation = new TimedInvocation(invocation);
            var beginTimedInvocation = new BeginTimedInvocation(timedInvocation);
            var jsonBeginTimedInvocation = JsonSerializer.Serialize(beginTimedInvocation);

            logger.Debug(jsonBeginTimedInvocation);

            var stopwatch = Stopwatch.StartNew();

            Exception exception = null;

            try
            {
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