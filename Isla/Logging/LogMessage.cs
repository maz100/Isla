using System;

namespace Isla.Logging
{
    public class LogMessage
    {
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public TimedInvocation TimedInvocation { get; set; }
    }
}