using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Isla.Logging
{
    public class TimedInvocation
    {
        private readonly InvocationSerialisation _invocationSerialisation;
        private object _returnValue;
        private object[] _arguments;

        public TimedInvocation()
        {
        }

        public TimedInvocation(IInvocation invocation, InvocationSerialisation invocationSerialisation = InvocationSerialisation.Both)
        {
            _invocationSerialisation = invocationSerialisation;
            MethodName = invocation.Method.Name;
            _arguments = invocation.Arguments;
            _returnValue = invocation.ReturnValue;
        }

        private object GetReturnValue(InvocationSerialisation invocationSerialisation, object returnValue)
        {
            if (returnValue != null && invocationSerialisation == InvocationSerialisation.None ||
                invocationSerialisation == InvocationSerialisation.Arguments)
            {
                return returnValue.GetType().ToString();
            }

            return returnValue;
        }

        private object[] GetArguments(InvocationSerialisation invocationSerialisation, object[] arguments)
        {
            if (arguments != null && (invocationSerialisation == InvocationSerialisation.None ||
                invocationSerialisation == InvocationSerialisation.ReturnValue))
            {
                return arguments.Select(x => (object)x.GetType().ToString()).ToArray();
            }

            return arguments;
        }

        public string MethodName { get; set; }

        public object[] Arguments
        {
            get { return GetArguments(_invocationSerialisation, _arguments); }
            set { _arguments = value; }
        }

        public object ReturnValue
        {
            get { return GetReturnValue(_invocationSerialisation, _returnValue); }
            set { _returnValue = value; }
        }

        public ExceptionInfo ExceptionInfo { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public class BeginTimedInvocation
    {
        public TimedInvocation Begin { get; set; }

        public BeginTimedInvocation(TimedInvocation invocation)
        {
            Begin = invocation;
        }
    }

    public class EndTimedInvocation
    {
        public TimedInvocation End { get; set; }

        public EndTimedInvocation(TimedInvocation invocation)
        {
            End = invocation;
        }
    }

    public enum InvocationSerialisation
    {
        None,
        Arguments,
        ReturnValue,
        Both
    }
}