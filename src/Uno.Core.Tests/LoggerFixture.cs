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
using System.Linq;
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
		public void TestNonGenericLogWithExternalLogger()
		{
			//setup
			var originalProvider = ServiceLocator.IsLocationProviderSet
				? ServiceLocator.Current
				: default;

			var fakeLocator = new FakeServiceLocator();

			ServiceLocator.SetLocatorProvider(() => fakeLocator);

			Assert.AreEqual(fakeLocator, ServiceLocator.Current);

			var message = "Test logging";

			typeof(string).Log().Debug(message);

			Assert.AreEqual(1, fakeLocator.Outputs.Count);

			var actualDebug = fakeLocator.Outputs.Single();
			Assert.AreEqual(message, actualDebug.Message);
			Assert.AreEqual(LogLevel.Debug, actualDebug.LogLevel);

			//ensure 'restore'
			ServiceLocator.SetLocatorProvider(() => originalProvider);
			Assert.AreEqual(originalProvider, ServiceLocator.Current);
		}

		[TestMethod]
		public void TestGenericLogWithExternalLogger()
		{
			//setup
			var originalProvider = ServiceLocator.IsLocationProviderSet
				? ServiceLocator.Current
				: default;

			var fakeLocator = new FakeServiceLocator();

			ServiceLocator.SetLocatorProvider(() => fakeLocator);

			Assert.AreEqual(fakeLocator, ServiceLocator.Current);

			var message = "Test logging";

			5.Log().Warn(message);

			Assert.AreEqual(1, fakeLocator.Outputs.Count);

			var actualWarning = fakeLocator.Outputs.Single();
			Assert.AreEqual(message, actualWarning.Message);
			Assert.AreEqual(LogLevel.Warning, actualWarning.LogLevel);

			//ensure 'restore'
			ServiceLocator.SetLocatorProvider(() => originalProvider);
			Assert.AreEqual(originalProvider, ServiceLocator.Current);
		}

		private class FakeServiceLocator : IServiceLocator
		{
			public IList<(LogLevel LogLevel, EventId EventId, string Message)> Outputs { get; } = new List<(LogLevel, EventId, string)>();

			public IEnumerable<object> GetAllInstances(Type serviceType) => throw new NotImplementedException();
			public IEnumerable<TService> GetAllInstances<TService>() => throw new NotImplementedException();
			public object GetInstance(Type serviceType) => throw new NotImplementedException();
			public object GetInstance(Type serviceType, string key) => throw new NotImplementedException();
			public TService GetInstance<TService>() => throw new NotImplementedException();
			public TService GetInstance<TService>(string key) => throw new NotImplementedException();
			public object GetService(Type serviceType)
			{
				if (serviceType.IsInterface && typeof(ILogger).IsAssignableFrom(serviceType)
					&& serviceType.GenericTypeArguments.Length == 1)
				{
					return Activator
						.CreateInstance(typeof(FakeLogger<>)
						.MakeGenericType(serviceType.GenericTypeArguments.Single()), args: this);
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
					locator.Outputs.Add((logLevel, eventId, formatter(state, exception)));
				}
			}
		}
	}
}