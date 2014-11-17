using System;

namespace Test.Isla.Serialisation.Components
{
    public class SomeClass : ISomeClass
    {
        #region ISomeClass implementation

        public string SomeMethod(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input", "parameter cannot be null or empty");
            }
            return input;
        }

        #endregion
    }
}