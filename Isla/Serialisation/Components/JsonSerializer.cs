using Isla.Serialisation.Blacklists;
using Newtonsoft.Json;

namespace Isla.Serialisation.Components
{
    public class JsonSerializer : IJsonSerializer
    {
        public IBlacklist Blacklist { get; set; }

        #region IJsonSerializer implementation

        public T Deserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public string Serialize(object instance)
        {
	        var serialisedInvocation = JsonConvert.SerializeObject(
		        instance,
		        new JsonSerializerSettings
			        {
				        ContractResolver = new BlacklistedContractResolver(Blacklist),
				        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				        PreserveReferencesHandling = PreserveReferencesHandling.Objects
			        });

            return serialisedInvocation;
        }

        #endregion
    }
}