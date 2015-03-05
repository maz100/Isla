using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Isla.Logging;
using Isla.Serialisation.Components;
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
        public bool Indent { get; set; }

        public static IslaInstaller With(InvocationSerialisation invocationSerialisation)
        {
            var installer = new IslaInstaller();

            IslaInstallerExtensions.With(installer, invocationSerialisation);

            return installer;   
        }

        public static IslaInstaller WithIndentation(bool indent)
        {
            var installer = new IslaInstaller();

            IslaInstallerExtensions.WithIndentation(installer, indent);

            return installer;
        }

        #region IWindsorInstaller implementation

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<JsonInvocationLoggingInterceptor>()
                    .DependsOn(Property.ForKey<InvocationSerialisation>().Eq(InvocationSerialisation)),
                Component.For<IJsonSerializer>()
                    .ImplementedBy<JsonSerializer>()
                    .DependsOn(Property.ForKey<bool>().Eq(Indent)),
                Classes.FromThisAssembly()
                    .Where(x => x.Namespace.Contains("Components"))
                    .WithServiceFirstInterface());
        }

        #endregion
    }

    public static class IslaInstallerExtensions
    {
        public static IslaInstaller With(this IslaInstaller installer, InvocationSerialisation invocationSerialisation)
        {
            installer.InvocationSerialisation = invocationSerialisation;

            return installer;
        }

        public static IslaInstaller WithIndentation(this IslaInstaller installer, bool indent)
        {
            installer.Indent = indent;

            return installer;
        }
    }
}