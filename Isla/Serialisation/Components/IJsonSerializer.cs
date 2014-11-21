using Isla.Logging;

namespace Isla.Serialisation.Components
{
    public interface IJsonSerializer
    {
        string Serialize(object instance);
        T Deserialize<T>(string source);
    }
}