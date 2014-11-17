using log4net;

namespace Isla.Logging.Components
{
    public class LogManager : ILogManager
    {
        #region ILogManager implementation

        public ILog GetLogger(string loggerName)
        {
            return log4net.LogManager.GetLogger(loggerName);
        }

        #endregion
    }
}