using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Isla.Logging;
using log4net.Config;

namespace Isla
{
    public class IslaInstaller : IWindsorInstaller
    {
        private static bool LogConfigured;

        public IslaInstaller()
        {
            if (!LogConfigured)
            {
                XmlConfigurator.Configure();
                LogConfigured = true;
            }
        }

        public InvocationSerialisation InvocationSerialisation { get; set; }

        public static IslaInstaller With(InvocationSerialisation invocationSerialisation)
        {
            var installer = new IslaInstaller();

            installer.InvocationSerialisation = invocationSerialisation;

            return installer;
        }

        #region IWindsorInstaller implementation

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<JsonInvocationLoggingInterceptor>()
                         .DependsOn(Property.ForKey<InvocationSerialisation>().Eq(InvocationSerialisation)),
                Classes.FromThisAssembly()
                    .Where(x => x.Namespace.Contains("Components"))
                    .WithServiceFirstInterface());
        }

        #endregion
    }
}