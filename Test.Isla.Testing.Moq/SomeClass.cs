namespace Test.Isla.Testing.Moq
{
    public class SomeClass : ISomeClass
    {
        private ISomeDependency1 _someDependency1;
        private ISomeDependency2 _someDependency2;

        public SomeClass(ISomeDependency1 someDependency1, ISomeDependency2 someDependency2)
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
}