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
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Conversion;
using Uno.Reflection;
using Funq;

namespace Uno.Core.Tests.Conversions
{
	[TestClass]
	public class EnumConversionStrategyFixture
	{
		[TestInitialize]
		public void TestInitialize()
		{
			var container = new Container();
			ServiceLocator.SetLocatorProvider(() => new FunqAdapter(container));

			container.Register<IReflectionExtensions>(c => new DefaultReflectionExtensions());
			container.Register<IConversionExtensions>(c => new DefaultConversionExtensions());
		}

		[TestCleanup]
		public void TearDown()
		{
			ServiceLocator.SetLocatorProvider(() => throw new InvalidOperationException("ServiceLocator provider not set."));
		}

		[TestMethod]
		public void EnumFromString()
		{
			Assert.AreEqual(Status.Verified, "Verified".Conversion().To<Status>());
			Assert.AreEqual(Status.Verified, "VER".Conversion().To<Status>());
		}

		[TestMethod]
		public void EnumFromUnknownString()
		{
			Assert.AreEqual(StatusWithUnkwown.Unknown, "Error".Conversion().To<StatusWithUnkwown>());
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void EnumFromInvalidStringThrowsInvalidCastException()
		{
			"Invalid".Conversion().To<Status>();
		}

		[TestMethod]
		public void StringFromEnum()
		{
			Assert.AreEqual("Cancelled", Status.Cancelled.Conversion().To<string>());
			Assert.AreEqual("VER", Status.Verified.Conversion().To<string>());
		}

		public enum Status
		{
			[System.ComponentModel.Description("VER")]
			Verified,
			Cancelled
		}

		public enum StatusWithUnkwown
		{
			[System.ComponentModel.Description("?")]
			Unknown,
			[System.ComponentModel.Description("VER")]
			Verified,
			Cancelled
		}
	}
}
