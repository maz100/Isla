using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Isla.Logging;

namespace Test.Isla
{
    public class TestInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller implementation

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                .Where(x => x.Namespace.Contains("Components"))
                .WithServiceFirstInterface()
                .Configure(x => x.Interceptors<JsonInvocationLoggingInterceptor>()));
        }

        #endregion
    }
}