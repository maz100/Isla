using System;

namespace Isla.Logging
{
    public class ExceptionInfo
    {
        public ExceptionInfo()
        {
        }

        public ExceptionInfo(Exception ex)
        {
            Message = ex.Message;
        }

        public string Message { get; set; }
    }
}