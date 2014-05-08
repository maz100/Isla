﻿using System;
using System.Linq;
using System.Reflection;
using Moq;

namespace Isla.Testing.Moq
{

	public static class AutoMockExtensions
	{
		public static Mock<T> Get<T>( this object instance ) where T : class
		{
			var propertyInfo = instance.GetType().GetProperties().FirstOrDefault( x => x.PropertyType == typeof( T ) );

			if( propertyInfo == null )
			{
				throw new MockInjectionException( "Could not locate mock of requested type: " + typeof( T ) );
			}

			//get the mocked interface
			var value = propertyInfo.GetValue( instance, null );

			//get its Mock property
			var property = value.GetType().GetProperties().FirstOrDefault( x => x.Name == "Mock" );

			if( property == null )
			{
				throw new MockInjectionException( "Could not locate mock of requested type: " + typeof( T ) );
			}

			return property.GetValue( value, null ) as Mock<T>;
		}

		public static void VerifyAll( this object instance )
		{
			PropertyInfo[] propertyInfos = instance.GetType().GetProperties();

			foreach( var propertyInfo in propertyInfos )
			{
				object value = propertyInfo.GetValue( instance, null );

				if (value == null)
				{
					continue;
				}

				//get its Mock property
				var property = value.GetType().GetProperties().FirstOrDefault( x => x.Name == "Mock" );

				if( property == null )
				{
					continue;
				}

				var mock = property.GetValue( value, null );

				Type[] genericArguments = mock.GetType().GetGenericArguments();

				try
				{
					mock.GetType().GetMethod( "VerifyAll", Type.EmptyTypes ).Invoke( mock, null );
				}
				catch( TargetInvocationException ex )
				{
					throw ex.InnerException;
				}
			}
		}
	}
}
