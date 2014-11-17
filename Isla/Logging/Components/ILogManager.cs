using log4net;

namespace Isla.Logging.Components
{
    public interface ILogManager
    {
        ILog GetLogger(string loggerName);
    }
}