using System;
using Castle.MicroKernel.Registration;
using Isla.Logging;

namespace Test.Isla
{
	public class TestInstaller : IWindsorInstaller
	{
		public TestInstaller ()
		{
		}

		#region IWindsorInstaller implementation

		public void Install (Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			container.Register (Classes.FromThisAssembly ()
				.Where (x => x.Namespace.Contains ("Components"))
				.WithServiceFirstInterface ()
				.Configure (x => x.Interceptors<JsonInvocationLoggingInterceptor> ()));
		}

		#endregion
	}
}

