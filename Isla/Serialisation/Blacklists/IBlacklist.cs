using System;
using System.Collections.Generic;

namespace Isla.Serialisation.Blacklists
{
	public interface IBlacklist
	{
		void AddType(Type type);
		bool ShouldNotSerialize(Type type);
		void AddProperty(string name);
		IList<string> GetProperties();
		bool Any();
	}
}
