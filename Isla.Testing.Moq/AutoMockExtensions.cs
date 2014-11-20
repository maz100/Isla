using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Isla.Logging;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using Moq;

namespace Isla.Testing.Moq
{
    public static class AutoMockExtensions
    {
        public static Mock<T> Mock<T>(this object instance) where T : class
        {
            Mock<T> mock;
            mock = GetMockFromProperty<T>(instance);

            if (mock != null) return mock;

            mock = GetMockFromPrivateField<T>(instance);

            return mock;
        }

        private static Mock<T> GetMockFromPrivateField<T>(object instance) where T : class
        {
            var type = instance.GetType().BaseType;

            var fieldInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.FieldType == typeof(T))
                .FirstOrDefault();

            if (fieldInfo == null)
            {
                throw new MockInjectionException("Could not locate mock of requested type: " + typeof(T));
            }

            var value = fieldInfo.GetValue(instance);

            //get its Mock property
            var property = value.GetType().GetProperties().FirstOrDefault(x => x.Name == "Mock");

            if (property == null)
            {
                //this gets the mock from a proxied instance
                var unproxiedInstance = Castle.DynamicProxy.ProxyUtil.GetUnproxiedInstance(value);
                property = unproxiedInstance.GetType().GetProperties().FirstOrDefault(x => x.Name == "Mock");
                value = unproxiedInstance;
            }

            if (property == null)
            {
                throw new MockInjectionException("Could not locate mock of requested type: " + typeof(T));
            }

            return property.GetValue(value, null) as Mock<T>;
        }

        private static Mock<T> GetMockFromProperty<T>(object instance) where T : class
        {
            if (ProxyUtil.IsProxy(instance))
            {
                //if the instance has been wrapped in a proxy, unwrap it
                instance = Castle.DynamicProxy.ProxyUtil.GetUnproxiedInstance(instance);
            }

            var propertyInfo = instance.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(T));

            if (propertyInfo == null)
            {
                return null;
            }

            //get the mocked interface
            var value = propertyInfo.GetValue(instance, null);

            if (ProxyUtil.IsProxy(value))
            {
                value = ProxyUtil.GetUnproxiedInstance(value);
            }
            
            //get its Mock property
            var property = value.GetType().GetProperties().FirstOrDefault(x => x.Name == "Mock");


            if (property == null)
            {
                throw new MockInjectionException("Could not locate mock of requested type: " + typeof(T));
            }

            return property.GetValue(value, null) as Mock<T>;
        }

        public static void VerifyAll(this object instance)
        {
            instance.Mocks().VerifyAll();
        }

        //creates a new mock object which will be included in the call to VerifyAll
        public static Mock<T> Create<T>(this object instance) where T : class
        {
            var mocksProvider = instance as IMockRepositoryProvider;
            return mocksProvider.Mocks().Create<T>();
        }

        public static MockRepository Mocks(this object instance)
        {
            if (ProxyUtil.IsProxy(instance))
            {
                //if the instance has been wrapped in a proxy, unwrap it
                instance = Castle.DynamicProxy.ProxyUtil.GetUnproxiedInstance(instance);
            }

            var mocksProvider = instance as IMockRepositoryProvider;

            return mocksProvider.Mocks();
        }

        public static MoqAutoMocker EnableLogging(this MoqAutoMocker instance)
        {
            var logger = new JsonInvocationLoggingInterceptor
            {
                JsonSerializer = new JsonSerializer(),
                LogManager = new LogManager()
            };

            instance.Interceptors.Add(logger);

            return instance;
        }

        public static MoqAutoMocker AddInterceptor(this MoqAutoMocker instance, IInterceptor interceptor)
        {
            instance.Interceptors.Add(interceptor);
            return instance;
        }
    }
}