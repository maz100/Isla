using System;
using System.Collections.Generic;
using System.Linq;
using Isla.Serialisation.Blacklists;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Isla.Serialisation.Components
{
    internal class BlacklistedContractResolver : DefaultContractResolver
    {
        private readonly IBlacklist _blacklist;

        internal BlacklistedContractResolver(IBlacklist blacklist)
        {
            _blacklist = blacklist;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            if (_blacklist == null)
            {
                return properties;
            }

            if (_blacklist.ShouldNotSerialize(type))
            {
                return new List<JsonProperty>();
            }

            if (!_blacklist.Any())
            {
                return properties;
            }

            return properties.Where(p => !_blacklist.GetProperties().Contains(p.PropertyName)).ToList();
        }
    }
}