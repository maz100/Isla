﻿using System;
using Castle.Services.Logging;
using Castle.MicroKernel.Registration;
using log4net.Config;
using Isla.Logging.Components;
using Isla.Logging;

namespace Isla
{
	public class IslaInstaller : IWindsorInstaller
	{
		public IslaInstaller ()
		{

		}

		#region IWindsorInstaller implementation
		public void Install (Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			XmlConfigurator.Configure ();

			container.Register(

				Component.For<JsonInvocationLoggingInterceptor>(),

				Classes.FromThisAssembly()
				.Where(x=>x.Namespace.Contains("Components"))
				.WithServiceFirstInterface());
		}
		#endregion
	}
}

