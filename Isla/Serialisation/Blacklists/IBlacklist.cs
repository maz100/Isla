using System.Collections.Generic;

namespace Isla.Serialisation.Blacklists
{
	public interface IBlacklist
	{
		void AddProperty(string name);
		IList<string> GetProperties();
		bool Any();
	}
}
