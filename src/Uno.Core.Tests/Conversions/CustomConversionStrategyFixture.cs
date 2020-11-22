// ******************************************************************
// Copyright � 2015-2020 nventive inc. All rights reserved.
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
using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Conversion;
using Uno.Reflection;
using Funq;

namespace Uno.Core.Tests.Conversions
{
	[TestClass]
	public class CustomConversionStrategyFixture
	{
		private Container _container;

		[TestInitialize]
		public void TestInitialize()
		{
			_container = new Container();
			ServiceLocator.SetLocatorProvider(() => new FunqAdapter(_container));

			_container.Register<IReflectionExtensions>(c => new DefaultReflectionExtensions());
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions(false));
		}

		[TestCleanup]
		public void TearDown()
		{
			ServiceLocator.SetLocatorProvider(() => throw new InvalidOperationException("ServiceLocator provider not set."));
		}

		[TestMethod]
		public void IntegrationTest()
		{
			_container.Resolve<IConversionExtensions>()
						.RegisterCustomStrategy<CustomType1, string>(
							input => input.Value);

			var from = new CustomType1 {Value = "123"};

			Assert.AreEqual("123", from.Conversion().To<string>());
		}

		private class CustomType1
		{
			internal string Value;
		}
	}
}
