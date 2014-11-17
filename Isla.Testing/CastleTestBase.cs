using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Isla.Testing
{
    public abstract class CastleTestBase
    {
        public CastleTestBase()
        {
            var container = new WindsorContainer();

            container.Install(Installers);

            foreach (var item in GetType().GetProperties())
            {
                var handlers = container.Kernel.GetAssignableHandlers(item.PropertyType);

                if (handlers.Length == 0)
                {
                    continue;
                }

                var dependencyInstance = container.Resolve(item.PropertyType);

                item.SetValue(this, dependencyInstance, null);
            }


            var constructors = GetType().GetConstructors();
        }

        public abstract IWindsorInstaller[] Installers { get; }
    }
}