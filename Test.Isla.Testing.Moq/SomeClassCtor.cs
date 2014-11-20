namespace Test.Isla.Testing.Moq
{
    /// <summary>
    /// Test implementation which supports constructor injection
    /// </summary>
    public class SomeClassCtor : ISomeClass
    {
        private ISomeDependency1 _someDependency1;
        private ISomeDependency2 _someDependency2;

        public SomeClassCtor(ISomeDependency1 someDependency1, ISomeDependency2 someDependency2)
        {
            _someDependency1 = someDependency1;
            _someDependency2 = someDependency2;
        }

        public string SomeMethod(string message)
        {
            _someDependency1.SomeMethod1();
            _someDependency2.SomeMethod2();

            return message;
        }
    }

    /// <summary>
    /// Test implementation which supports property injection
    /// </summary>
    public class SomeClassProp : ISomeClass
    {
        public ISomeDependency1 SomeDependency1 { get; set; }
        public ISomeDependency2 SomeDependency2 { get; set; }

        public string SomeMethod(string message)
        {
            SomeDependency1.SomeMethod1();
            SomeDependency2.SomeMethod2();

            return message;
        }
    }
}