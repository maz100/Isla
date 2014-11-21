using System;
using Castle.DynamicProxy;

namespace Isla.Logging
{
    public class TimedInvocation
    {
        public TimedInvocation()
        {
        }

        public TimedInvocation(IInvocation invocation)
        {
            MethodName = invocation.Method.Name;
            Arguments = invocation.Arguments;
            ReturnValue = invocation.ReturnValue;
        }

        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public object ReturnValue { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public class BeginTimedInvocation
    {
        public TimedInvocation Begin { get; set; }

        public BeginTimedInvocation(IInvocation invocation)
        {
            Begin = new TimedInvocation(invocation);
        }
    }

    public class EndTimedInvocation
    {
        public TimedInvocation End { get; set; }

        public EndTimedInvocation(IInvocation invocation)
        {
            End = new TimedInvocation(invocation);
        }
    }
}