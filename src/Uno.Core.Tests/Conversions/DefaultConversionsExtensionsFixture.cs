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
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Conversion;
using Uno.Reflection;
using SystemDescription = System.ComponentModel.DescriptionAttribute;
using Funq;

namespace Uno.Core.Tests.Conversions
{
	[TestClass]
	public class DefaultConversionsExtensionsFixture
	{
		private Container _container;

		private enum DummyEnum : byte
		{
			Value1,

			[SystemDescription("Value2Description")]
			Value2
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_container = new Container();
			ServiceLocator.SetLocatorProvider(() => new FunqAdapter(_container));

			_container.Register<IReflectionExtensions>(c => new DefaultReflectionExtensions());
		}

		[TestCleanup]
		public void TearDown()
		{
			ServiceLocator.SetLocatorProvider(() => throw new InvalidOperationException("ServiceLocator provider not set."));
		}

		[TestMethod]
		public void TestWithEnumToStringStrategy()
		{
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions());

			var result1 = DummyEnum.Value1.Conversion().To<string>();
			var result2 = DummyEnum.Value2.Conversion().To<string>();

			Assert.AreEqual("Value1", result1);
			Assert.AreEqual("Value2Description", result2);
		}

		[TestMethod]
		public void TestWithStringToEnumStrategy()
		{
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions());

			var result1 = "Value1".Conversion().To<DummyEnum>();
			var result2 = "vAlUe1".Conversion().To<DummyEnum>();
			var result3 = "Value2".Conversion().To<DummyEnum>();
			var result4 = "Value2Description".Conversion().To<DummyEnum>();
			var result5 = "vAlUe2dEsCrIpTiOn".Conversion().To<DummyEnum>();
			var result6 = "    vAlUe2dEsCrIpTiOn    ".Conversion().To<DummyEnum>();

			Assert.AreEqual(DummyEnum.Value1, result1);
			Assert.AreEqual(DummyEnum.Value1, result2);
			Assert.AreEqual(DummyEnum.Value2, result3);
			Assert.AreEqual(DummyEnum.Value2, result4);
			Assert.AreEqual(DummyEnum.Value2, result5);
			Assert.AreEqual(DummyEnum.Value2, result6);
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void TestWithStringToEnumStrategyWhenInvalidInput()
		{
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions());

			"InvalidValue-XXX".Conversion().To<DummyEnum>();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestWithEnumStrategyWhenNoStrategiesRegistered()
		{
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions(false));

			DummyEnum.Value1.Conversion().To<string>();
		}

		[TestMethod]
		public void TestWithCustomRegistration()
		{
			_container.Register<IConversionExtensions>(c => new DefaultConversionExtensions(false));

			var sut = _container.Resolve<IConversionExtensions>();
			sut.RegisterStrategy<CustomStrategy>();

			var result = DummyEnum.Value1.Conversion().To<string>();

			Assert.AreEqual("[String] Value1", result);
		}

		public class CustomStrategy : IConversionStrategy
		{
			public bool CanConvert(object value, Type toType, CultureInfo culture = null)
			{
				return true;
			}

			public object Convert(object value, Type toType, CultureInfo culture = null)
			{
				return string.Format("[{0}] {1}", toType.Name, value);
			}
		}
	}
}
