using System.Collections.Generic;

namespace Isla.Logging.Components
{
    public interface IJsonLogReader
    {
        IEnumerable<LogMessage> GetLogMessages(string path);
    }
}