using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Isla.Logging;
using Isla.Logging.Components;
using Isla.Serialisation.Components;
using Moq;

namespace Isla.Testing.Moq
{
    public class MoqAutoMocker
    {
        private IList<IInterceptor> _interceptors;


        public TInterface ProxyInstance<TInterface, TClass>()
            where TClass : class, TInterface
            where TInterface : class
        {
            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();

            var instance = CreateInstance<TClass>(Interceptors.ToArray());

            var result = generator.CreateInterfaceProxyWithTarget<TInterface>(instance, options, Interceptors.ToArray());

            return result;
        }

        public static T CreateInstance<T>() where T : class
        {
            return CreateInstance<T>(new IInterceptor[0]);
        }

        public static T CreateInstance<T>(params IInterceptor[] interceptors) where T : class
        {
            var mockRepositoryProvider = new MockRepositoryProvider();
            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(mockRepositoryProvider);

            var typeToInstantiate = typeof(T);
            var constructors = typeToInstantiate.GetConstructors();
            var nonDefaultCtorCount = constructors.Count(x => x.GetParameters().Length > 0);

            if (nonDefaultCtorCount > 1)
            {
                throw new MockInjectionException("Class has multiple non-default constructors, what to do?");
            }

            var ctorCount = constructors.Count();

            var defaultCtor = constructors.FirstOrDefault(x => x.GetParameters().Length == 0);

            var nonDefaultCtor = constructors.FirstOrDefault(x => x.GetParameters().Length != 0);

            var useDefaultCtor = ctorCount == 1 && defaultCtor != null;

            T instance;

            if (useDefaultCtor)
            {
                instance = (T)generator.CreateClassProxy(typeof(T), options, interceptors);

                //if there are no constructor args, try property injection
                InjectProperties<T>(instance, interceptors);
            }
            else
            {
                instance = InjectNonDefaultConstructor<T>(nonDefaultCtor, mockRepositoryProvider, generator, options, interceptors);
            }

            return instance;
        }

        private static T InjectNonDefaultConstructor<T>(ConstructorInfo nonDefaultCtor, MockRepositoryProvider mockRepositoryProvider, ProxyGenerator generator, ProxyGenerationOptions options, IInterceptor[] interceptors) where T : class
        {
            if (interceptors == null)
            {
                interceptors = new IInterceptor[0];
            }
            var ctorParams = nonDefaultCtor.GetParameters();

            var mocks = mockRepositoryProvider.Mocks();

            var ctorArgValues = new object[ctorParams.Length];

            for (var i = 0; i < ctorParams.Length; i++)
            {
                dynamic mock = InvocationHelper.InvokeGenericMethodWithDynamicTypeArguments(mocks, a => a.Create<object>(),
                                                                                         null,
                                                                                         ctorParams[i].ParameterType);

                dynamic proxiedMock = generator.CreateInterfaceProxyWithTarget(ctorParams[i].ParameterType, mock.Object, interceptors);

                ctorArgValues[i] = proxiedMock;
            }

            var instance = (T)generator.CreateClassProxy(typeof(T), options, ctorArgValues, interceptors);

            return instance;
        }

        private static void InjectProperties<T>(object instance, params IInterceptor[] interceptors)
        {
            var propertyInfos = typeof(T).GetProperties().Where(x => x.PropertyType.IsInterface);

            foreach (var propertyInfo in propertyInfos)
            {
                InjectPropertyWithMock(propertyInfo, instance, interceptors);
            }
        }

        private static void InjectPropertyWithMock(PropertyInfo propertyInfo, object instance, params IInterceptor[] interceptors)
        {
            if (!propertyInfo.CanWrite)
            {
                return;
            }

            var myType = propertyInfo.PropertyType;

            var mocks = instance.Mocks();

            var mock = InvocationHelper.InvokeGenericMethodWithDynamicTypeArguments(mocks, a => a.Create<object>(), null,
                myType);

            var generator = new ProxyGenerator();

            var proxiedMock = generator.CreateInterfaceProxyWithTarget(myType, ((Mock)mock).Object, interceptors);

            propertyInfo.SetValue(instance, proxiedMock, null);
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

        public IList<IInterceptor> Interceptors
        {
            get { return _interceptors ?? (_interceptors = new List<IInterceptor>()); }
            set { _interceptors = value; }
        }

        public static MoqAutoMocker Configure()
        {
            return new MoqAutoMocker();
        }
    }
}