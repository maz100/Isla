using System;
using System.Collections.Generic;
using System.Linq;

namespace Isla.Serialisation.Blacklists
{
    public class DefaultBlacklist : IBlacklist
    {
        private ISet<string> _blacklistedPropertyNames = new HashSet<string>();

        #region IBlacklist implementation

        public void AddType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool ShouldNotSerialize(Type type)
        {
            return false;
        }

        public void AddProperty(string name)
        {
            _blacklistedPropertyNames.Add(name);
        }

        public IList<string> GetProperties()
        {
            return _blacklistedPropertyNames.ToList();
        }

        public bool Any()
        {
            return _blacklistedPropertyNames.Any();
        }

        #endregion
    }
}

