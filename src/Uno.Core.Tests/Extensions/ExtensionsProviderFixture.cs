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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests.Extensions
{
	[TestClass]
	public class ExtensionsProviderFixture
	{
		[TestCleanup]
		public void TearDown()
		{
			ExtensionsProvider.ServiceProvider = null;
		}

		[TestMethod]
		public void When_No_ServiceProvider_Then_Default_Is_Used()
		{
			Assert.IsInstanceOfType(ExtensionsProvider.Get<IService, DefaultService>(), typeof(DefaultService));
			Assert.IsInstanceOfType(ExtensionsProvider.Get<IService>(() => new DefaultService()), typeof(DefaultService));
		}

		[TestMethod]
		public void When_Service_Is_Not_Provided_Then_Default_Is_Used()
		{
			ExtensionsProvider.ServiceProvider = new StubServiceProvider(null);

			Assert.IsInstanceOfType(ExtensionsProvider.Get<IService, DefaultService>(), typeof(DefaultService));
			Assert.IsInstanceOfType(ExtensionsProvider.Get<IService>(() => new DefaultService()), typeof(DefaultService));
		}

		[TestMethod]
		public void When_Service_Is_Provided_Then_It_Is_Used()
		{
			var provided = new CustomService();
			ExtensionsProvider.ServiceProvider = new StubServiceProvider(provided);

			Assert.AreSame(provided, ExtensionsProvider.Get<IService, DefaultService>());
			Assert.AreSame(provided, ExtensionsProvider.Get<IService>(() => new DefaultService()));
		}

		[TestMethod]
		public void When_Service_Is_Of_Wrong_Type_Then_Throws()
		{
			ExtensionsProvider.ServiceProvider = new StubServiceProvider(new object());

			Assert.ThrowsException<InvalidOperationException>(() => ExtensionsProvider.Get<IService, DefaultService>());
			Assert.ThrowsException<InvalidOperationException>(() => ExtensionsProvider.Get<IService>(() => new DefaultService()));
		}

		private interface IService
		{
		}

		private class DefaultService : IService
		{
		}

		private class CustomService : IService
		{
		}

		private class StubServiceProvider : IServiceProvider
		{
			private readonly object _service;

			public StubServiceProvider(object service)
			{
				_service = service;
			}

			public object GetService(Type serviceType) => _service;
		}
	}
}
