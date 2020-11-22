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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class EnumerableExtensions_FullOuterJoin
	{
		[TestMethod]
		public async Task TestBasicResult()
		{
			var left = GetLeft(0, 1, 2, 3);
			var right = GetLeft(0, 1, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			Assert.AreEqual(4, results.Length, "Count");
			CollectionAssert.AreEqual(left, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(right, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedKeyLeft()
		{
			var left = GetLeft(0, 1, 2, 2, 3);
			var right = GetLeft(0, 1, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = left;
			var expectedRight = new []
			{
				right[0],
				right[1],
				right[2],
				right[2],
				right[3],
			};

			Assert.AreEqual(5, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedKeyRight()
		{
			var left = GetLeft(0, 1, 2, 3);
			var right = GetLeft(0, 1, 2, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				left[2],
				left[3],
			};
			var expectedRight = right;

			Assert.AreEqual(5, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedKeyLeftAndRight()
		{
			var left = GetLeft(0, 1, 1, 2, 3);
			var right = GetLeft(0, 1, 2, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				left[3],
				left[3],
				left[4],
			};
			var expectedRight = new[]
			{
				right[0],
				right[1],
				right[1],
				right[2],
				right[3],
				right[4],
			};

			Assert.AreEqual(6, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithMissingLeft()
		{
			var left = GetLeft(0, 1, 3);
			var right = GetLeft(0, 1, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				null,
			};
			var expectedRight = new[]
			{
				right[0],
				right[1],
				right[3],
				right[2],
			};

			Assert.AreEqual(4, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithMissingRight()
		{
			var left = GetLeft(0, 1, 2, 3);
			var right = GetLeft(0, 1, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				left[3],
			};
			var expectedRight = new[]
			{
				right[0],
				right[1],
				null,
				right[2],
			};

			Assert.AreEqual(4, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}


		[TestMethod]
		public async Task Test_WithMissingLeftAndRight()
		{
			var left = GetLeft(0, 2, 3);
			var right = GetLeft(0, 1, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				null,
			};
			var expectedRight = new[]
			{
				right[0],
				null,
				right[2],
				right[1],
			};

			Assert.AreEqual(4, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedMissingLeft()
		{
			var left = GetLeft(0, 1, 3);
			var right = GetLeft(0, 1, 2, 2, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				null,
				null,
			};
			var expectedRight = new[]
			{
				right[0],
				right[1],
				right[4],
				right[2],
				right[3],
			};

			Assert.AreEqual(5, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedMissingRight()
		{
			var left = GetLeft(0, 1, 2, 2, 3);
			var right = GetLeft(0, 1, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				left[3],
				left[4],
			};
			var expectedRight = new[]
			{
				right[0],
				right[1],
				null,
				null,
				right[2],
			};

			Assert.AreEqual(5, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		[TestMethod]
		public async Task Test_WithDuplicatedMissingLeftAndRight()
		{
			var left = GetLeft(0, 2, 2, 3);
			var right = GetLeft(0, 1, 1, 3);

			var results = EnumerableExtensions
				.FullOuterJoin(
					left,
					right,
					l => l.Key,
					r => r.Key,
					(l, r) => new
					{
						left = l,
						right = r
					})
				.ToArray();

			var expectedLeft = new[]
			{
				left[0],
				left[1],
				left[2],
				left[3],
				null,
				null,
			};
			var expectedRight = new[]
			{
				right[0],
				null,
				null,
				right[3],
				right[1],
				right[2],
			};

			Assert.AreEqual(6, results.Length, "Count");
			CollectionAssert.AreEqual(expectedLeft, results.Select(r => r.left).ToArray(), "Left");
			CollectionAssert.AreEqual(expectedRight, results.Select(r => r.right).ToArray(), "Right");
		}

		private static LeftItem[] GetLeft(params int[] keys)
		{
			var keyCount = new int[keys.Max() + 1];

			return keys.Select(i => new LeftItem(i, $"Left_Value_{i}_{keyCount[i]++}")).ToArray();
		}

		private static RightItem[] GetRight(params int[] keys)
		{
			var keyCount = new int[keys.Max() + 1];

			return keys.Select(i => new RightItem(i, $"Right_Value_{i}_{keyCount[i]++}")).ToArray();
		}

		private class LeftItem
		{
			public LeftItem(int key, string value)
			{
				Key = key;
				Value = value;
			}

			public int Key { get; }
			public string Value { get; }
		}

		private class RightItem
		{
			public RightItem(int key, string value)
			{
				Key = key;
				Value = value;
			}

			public int Key { get; }
			public string Value { get; }
		}
	}
}
