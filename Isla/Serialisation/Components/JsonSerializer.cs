﻿using Isla.Logging;
using Newtonsoft.Json;

namespace Isla.Serialisation.Components
{
    public class JsonSerializer : IJsonSerializer
    {
        #region IJsonSerializer implementation

        public T Deserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public string Serialize(object instance)
        {
            var serialisedInvocation = JsonConvert.SerializeObject(instance);

            return serialisedInvocation;
        }

        #endregion
    }
}