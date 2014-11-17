using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using Moq;

namespace Isla.Testing.Moq
{
    public class MoqAutoMocker
    {
        public static T CreateInstance<T>() where T : new()
        {
            var mockRepositoryProvider = new MockRepositoryProvider();
            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(mockRepositoryProvider);

            var instance = (T) generator.CreateClassProxy(typeof (T), null, options);

            var propertyInfos = typeof (T).GetProperties().Where(x => x.PropertyType.IsInterface);

            foreach (var propertyInfo in propertyInfos)
            {
                InjectPropertyWithMock(propertyInfo, instance);
            }

            return instance;
        }

        private static void InjectPropertyWithMock(PropertyInfo propertyInfo, object instance)
        {
            var myType = propertyInfo.PropertyType;

            var mocks = instance.Mocks();

            var mock = InvocationHelper.InvokeGenericMethodWithDynamicTypeArguments(mocks, a => a.Create<object>(), null,
                myType);

            if (!propertyInfo.CanWrite)
            {
                return;
            }

            propertyInfo.SetValue(instance, ((Mock) mock).Object, null);
        }

        public static class InvocationHelper
        {
            public static object InvokeGenericMethodWithDynamicTypeArguments<T>(T target,
                Expression<Func<T, object>> expression, object[] methodArguments, params Type[] typeArguments)
            {
                var methodInfo = ReflectionHelper.GetMethod(expression);
                if (methodInfo.GetGenericArguments().Length != typeArguments.Length)
                    throw new ArgumentException(
                        string.Format("The method '{0}' has {1} type argument(s) but {2} type argument(s) were passed.",
                            methodInfo.Name,
                            methodInfo.GetGenericArguments().Length,
                            typeArguments.Length));

                return methodInfo
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeArguments)
                    .Invoke(target, methodArguments);
            }
        }
    }
}