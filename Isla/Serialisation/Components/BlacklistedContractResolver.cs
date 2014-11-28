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
		private readonly IBlacklist blacklist;

		internal BlacklistedContractResolver(IBlacklist blacklist)
		{
			this.blacklist = blacklist;
		}		
		
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
			if (this.blacklist == null)
			{
				return properties;
			}

			if (this.blacklist.ShouldNotSerialize(type))
			{
				return new List<JsonProperty>();
			}

			if (!this.blacklist.Any())
			{
				return properties;
			}

			return properties.Where(p => !this.blacklist.GetProperties().Contains(p.PropertyName)).ToList();
		}
	}
}