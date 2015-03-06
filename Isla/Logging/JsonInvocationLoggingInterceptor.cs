using Castle.DynamicProxy;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using L4nLogManager = log4net.LogManager;

namespace Isla.Logging
{
    public class JsonInvocationLoggingInterceptor : IInterceptor
    {
        public IJsonSerializer JsonSerializer { get; set; }
        public ILogManager LogManager { get; set; }
        public InvocationSerialisation InvocationSerialisation { get; set; }

        private Exception exception;
        private Stopwatch stopwatch;
        private TimedInvocation timedInvocation;
        private ILog logger;

        #region IInterceptor implementation

        public void Intercept(IInvocation invocation)
        {
            BeforeProceed(invocation);

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
                AfterProceed(invocation, exception);
            }
        }

        private void AfterProceed(IInvocation invocation, Exception exception)
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

        private void BeforeProceed(IInvocation invocation)
        {
            if (!L4nLogManager.GetRepository().Configured)
            {
                XmlConfigurator.Configure();
            }

            logger = LogManager.GetLogger(invocation.InvocationTarget.GetType().Name);

            if (log4net.ThreadContext.Properties["callid"] == null)
            {
                log4net.ThreadContext.Properties["callid"] = Guid.NewGuid();
            }

            timedInvocation = new TimedInvocation(invocation, InvocationSerialisation);
            var beginTimedInvocation = new BeginTimedInvocation(timedInvocation);
            var jsonBeginTimedInvocation = JsonSerializer.Serialize(beginTimedInvocation);

            logger.Debug(jsonBeginTimedInvocation);

            stopwatch = Stopwatch.StartNew();
        }

        #endregion
    }
}