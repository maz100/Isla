using System;
using Castle.MicroKernel.Registration;
using log4net.Config;
using Isla.Logging.Components;
using Isla.Logging;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;

namespace Isla
{
	public class IslaInstaller : IWindsorInstaller
	{
		public IslaInstaller ()
		{
			if (!LogConfigured) {
				XmlConfigurator.Configure ();
				LogConfigured = true;
			}
		}

		private static bool LogConfigured;

		#region IWindsorInstaller implementation

		public void Install (IWindsorContainer container, IConfigurationStore store)
		{
			container.Register (

				Component.For<JsonInvocationLoggingInterceptor> (),

				Classes.FromThisAssembly ()
				.Where (x => x.Namespace.Contains ("Components"))
				.WithServiceFirstInterface ());
		}

		#endregion
	}
}

