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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Logging;

namespace Uno.Core.Tests
{
	[TestClass]
	public class LoggerFixtures
	{
		[TestMethod]
		public void TestInfo()
		{
			this.Log().Info("Test logging");
		}
		[TestMethod]
		public void TestWarn()
		{
			this.Log().Warn("Test logging");
		}
		[TestMethod]
		public void TestError()
		{
			this.Log().Error("Test logging");
		}
		[TestMethod]
		public void TestDebug()
		{
			this.Log().Debug("Test logging");
		}


		[TestMethod]
		public void TestWithExternalLogger()
		{
			//setup
			var originalProvider = ServiceLocator.IsLocationProviderSet
				? ServiceLocator.Current
				: default;

			var fakeLocator = new FakeServiceLocator();

			ServiceLocator.SetLocatorProvider(() => fakeLocator);

			Assert.AreEqual(fakeLocator, ServiceLocator.Current);

			var message = "Test logging";

			this.Log().Warn(message);
			this.Log().Debug(message);

			Assert.AreEqual(2, fakeLocator.Outputs.Count);

			var actualWarning = fakeLocator.Outputs[0];
			Assert.AreEqual(message, actualWarning.State?.ToString());
			Assert.AreEqual(LogLevel.Warning, actualWarning.LogLevel);

			var actualDebug = fakeLocator.Outputs[1];
			Assert.AreEqual(actualDebug, actualWarning.State?.ToString());
			Assert.AreEqual(LogLevel.Debug, actualWarning.LogLevel);

			//ensure 'restore'
			ServiceLocator.SetLocatorProvider(() => originalProvider);
			if (originalProvider == null)
			{
				Assert.IsFalse(ServiceLocator.IsLocationProviderSet);
			}
			else
			{
				Assert.AreEqual(originalProvider, ServiceLocator.Current);
			}
		}

		private class FakeServiceLocator : IServiceLocator
		{
			public IList<(LogLevel LogLevel, EventId EventId, object State, Exception Exception)> Outputs { get; } = new List<(LogLevel, EventId, object, Exception)>();

			public IEnumerable<object> GetAllInstances(Type serviceType) => throw new NotImplementedException();
			public IEnumerable<TService> GetAllInstances<TService>() => throw new NotImplementedException();
			public object GetInstance(Type serviceType) => throw new NotImplementedException();
			public object GetInstance(Type serviceType, string key) => throw new NotImplementedException();
			public TService GetInstance<TService>() => throw new NotImplementedException();
			public TService GetInstance<TService>(string key) => throw new NotImplementedException();
			public object GetService(Type serviceType)
			{
				if (serviceType.IsInterface && typeof(ILogger).IsAssignableFrom(serviceType) && serviceType.IsGenericType && serviceType.GenericTypeArguments.Length == 1)
				{
					return Activator.CreateInstance(typeof(FakeLogger<>).MakeGenericType(serviceType.GenericTypeArguments.Single()), this);
				}
				else
				{
					throw new NotImplementedException();
				}
			}

			class FakeLogger<T> : ILogger<T>
			{
				readonly FakeServiceLocator locator;
				public FakeLogger(FakeServiceLocator locator)
				{
					this.locator = locator;
				}

				public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
				public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();
				public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
				{
					locator.Outputs.Add((logLevel, eventId, state, exception));
				}
			}


		}

	}
}