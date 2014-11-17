using Castle.MicroKernel.Registration;
using Isla;
using Isla.Components;
using Isla.Testing;
using NUnit.Framework;

namespace Test.Isla.Testing
{
    [TestFixture]
    public class CastleTestBaseTests : CastleTestBase
    {
        public IFileHelper FileHelper { get; set; }

        #region implemented abstract members of CastleTestBase

        public override IWindsorInstaller[] Installers
        {
            get { return new[] {new IslaInstaller()}; }
        }

        [Test]
        public void TestFileHelperIsNotNull()
        {
            Assert.IsNotNull(FileHelper);
        }

        #endregion
    }
}