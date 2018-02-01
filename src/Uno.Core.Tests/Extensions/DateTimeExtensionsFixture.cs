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

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class DateTimeExtensionsFixture
	{
		public class Customer
		{
			public string City { get; set; }
		}

		[TestMethod]
		public void IsWeekDay()
		{
			Assert.IsTrue(new DateTime(2008, 2, 1).IsWeekDay());
			Assert.IsFalse(new DateTime(2008, 2, 2).IsWeekDay());
		}

		[TestMethod]
		public void IsWeekEnd()
		{
			Assert.IsFalse(new DateTime(2008, 2, 1).IsWeekEnd());
			Assert.IsTrue(new DateTime(2008, 2, 2).IsWeekEnd());
		}

		[TestMethod]
		public void Equal()
		{
			var x = new DateTime(2008, 2, 1);
			var y = new DateTime(2008, 2, 2);

			Assert.IsTrue(x.Equal(y, DateTimeUnit.ToMonth));
			Assert.IsFalse(x.Equal(y, DateTimeUnit.ToDay));
		}

		[TestMethod]
		public void Truncate()
		{
			Assert.AreEqual(new DateTime(2008, 2, 1), new DateTime(2008, 2, 2).Truncate(DateTimeUnit.ToMonth));
		}
	}
}
