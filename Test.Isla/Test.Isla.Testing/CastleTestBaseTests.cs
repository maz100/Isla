using System;
using Isla.Testing;
using Castle.MicroKernel.Registration;
using Isla;
using NUnit.Framework;
using Isla.Components;

namespace Test.Isla.Testing
{
	[TestFixture]
	public class CastleTestBaseTests : CastleTestBase
	{
		public IFileHelper FileHelper { get; set; }

		public CastleTestBaseTests ()
		{
		}

		#region implemented abstract members of CastleTestBase

		public override IWindsorInstaller[] Installers {
			get {
				return new []{ new IslaInstaller () };
			}
		}

		[Test]
		public void TestFileHelperIsNotNull ()
		{
			Assert.IsNotNull (FileHelper);
		}

		#endregion
	}
}

