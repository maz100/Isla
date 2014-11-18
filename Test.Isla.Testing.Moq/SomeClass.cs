namespace Test.Isla.Testing.Moq
{
    public class SomeClass
    {
        private ISomeDependency1 _someDependency1;
        private ISomeDependency2 _someDependency2;

        public SomeClass(ISomeDependency1 someDependency1, ISomeDependency2 someDependency2)
        {
            _someDependency1 = someDependency1;
            _someDependency2 = someDependency2;
        }

        public void SomeMethod()
        {
            _someDependency1.SomeMethod1();
            _someDependency2.SomeMethod2();
        }
    }
}