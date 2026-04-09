// ******************************************************************
// Copyright � 2015-2018 nventive inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// ******************************************************************
using System;
using System.Threading;
using CommonServiceLocator;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Uno.Extensions
{
    public static class LogExtensionPoint
    {
        private static ILoggerFactory? _loggerFactory;
        private static Func<ILoggerFactory?, ILoggerFactory, ILoggerFactory>? _factoryInterceptor;

		private static class Container<T>
        {
            internal static readonly ILogger Logger = AmbientLoggerFactory.CreateLogger<T>();
        }

		/// <summary>
		/// Retreives the <see cref="ILoggerFactory"/> for this the Uno extension point.
		/// </summary>
		public static ILoggerFactory AmbientLoggerFactory
		{
			get => Transactional.Update(ref _loggerFactory, l => l ?? GetFactory())!;
			set
			{
				// Avoid Transactional.Update for the interceptor path because its selector
				// can run multiple times under contention, causing the interceptor to fire
				// repeatedly and potentially allocate duplicate factories/providers.
				var interceptor = _factoryInterceptor;
				var resolved = interceptor is not null ? interceptor(Volatile.Read(ref _loggerFactory), value) : value;
				Volatile.Write(ref _loggerFactory, resolved);
			}
		}

		/// <summary>
		/// Registers an optional interceptor that is invoked whenever <see cref="AmbientLoggerFactory"/>
		/// is set. The callback receives the current factory and the proposed new factory, and must
		/// return a non-null factory that will be stored. This allows a host (e.g. Studio Live) to wrap
		/// or replace the factory before it takes effect — ensuring forwarding providers and filter
		/// overrides are applied before any loggers are created from the new factory.
		/// </summary>
		/// <param name="interceptor">
		/// A function that receives (currentFactory, proposedFactory) and returns the factory to use.
		/// The return value must not be null. Pass <c>null</c> to remove a previously registered interceptor.
		/// </param>
		/// <returns>
		/// An <see cref="IDisposable"/> that, when disposed, removes the interceptor and restores
		/// <see cref="AmbientLoggerFactory"/> to the value it held at the time of registration.
		/// Disposing the token when <paramref name="interceptor"/> is <c>null</c> is a no-op.
		/// </returns>
		public static IDisposable RegisterFactoryInterceptor(Func<ILoggerFactory?, ILoggerFactory, ILoggerFactory>? interceptor)
		{
			if (interceptor is null)
			{
				_factoryInterceptor = null;
				return NullDisposable.Instance;
			}

			var previousFactory = Volatile.Read(ref _loggerFactory);
			_factoryInterceptor = interceptor;
			return new InterceptorRegistration(previousFactory);
		}

		private sealed class NullDisposable : IDisposable
		{
			internal static readonly NullDisposable Instance = new NullDisposable();
			public void Dispose() { }
		}

		private sealed class InterceptorRegistration : IDisposable
		{
			private readonly ILoggerFactory? _previousFactory;
			private int _disposed;

			internal InterceptorRegistration(ILoggerFactory? previousFactory)
			{
				_previousFactory = previousFactory;
			}

			public void Dispose()
			{
				if (Interlocked.Exchange(ref _disposed, 1) == 0)
				{
					_factoryInterceptor = null;
					Volatile.Write(ref _loggerFactory, _previousFactory);
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="ILogger"/> for the specified type.
		/// </summary>
		/// <param name="forType"></param>
		/// <returns></returns>
		public static ILogger Log(this Type forType) 
			=> AmbientLoggerFactory.CreateLogger(forType);

		/// <summary>
		/// Gets a logger instance for the current types
		/// </summary>
		/// <typeparam name="T">The type for which to get the logger</typeparam>
		/// <param name="instance"></param>
		/// <returns>A logger for the type of the instance</returns>
		public static ILogger Log<T>(this T instance) => Container<T>.Logger;

		private static ILoggerFactory GetFactory()
		{
			if (ServiceLocator.IsLocationProviderSet)
			{
				try
				{
					var service = ServiceLocator.Current.GetService(typeof(ILoggerFactory));

					if (service is ILoggerFactory factory)
					{
						return factory;
					}

					throw new InvalidOperationException($"The service {service?.GetType()} is not of type ILoggerFactory.");
				}
				catch (Exception e)
				{
					if (e is NullReferenceException || e is InvalidOperationException)
					{
#if HAS_CONSOLE
						Console.WriteLine("***** WARNING *****");
						Console.WriteLine("Unable to get the service locator ({0}), using the default logger to System.Diagnostics.Debug", e.Message);
#endif
						return new LoggerFactory();
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				return new LoggerFactory();
			}
		}
	}
}
