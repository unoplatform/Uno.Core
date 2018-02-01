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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions;

namespace Umbrella.UI.Tests.Common
{
	[TestClass]
	public class CollectionExtensions_RangeList_Fixture
	{
		[TestMethod]
		public void When_SameLength()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(0, 3);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public void When_GreaterLength()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(0, 4);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public void When_LesserLength()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(0, 2);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(2, result[1]);
		}

		[TestMethod]
		public void When_SkipEqual()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(3, 1);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void When_SkipLesser()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(2, 1);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(3, result[0]);
		}

		[TestMethod]
		public void When_SkipLesser_TakeLarger()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(2, 2);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(3, result[0]);
		}

		[TestMethod]
		public void When_SkipGreater()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(4, 1);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void When_None()
		{
			var result = new List<int> { 1, 2, 3 }.ToRangeList(0, 0);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void When_SkipGreater_TakeGreater()
		{
			var result = new List<int> { 1, }.ToRangeList(10, 10);

			Assert.AreEqual(0, result.Count);
		}
	}
}
