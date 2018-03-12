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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Collections;

namespace Uno.Core.Tests.Collections
{
	[TestClass]
	public class ImmutableListFixture
	{
		[TestMethod]
		public void ImmutableList_Empty()
		{
			var sut = new ImmutableList<int>();

			sut.Should().HaveCount(0);
		}

		[TestMethod]
		public void ImmutableList_Construction_ShouldBeImmutable()
		{
			var items = new[] {1, 2, 3, 1, 2, 3};
			var sut = new ImmutableList<int>(items, copyData: true);

			sut.Should().BeEquivalentTo(new[] {1, 2, 3, 1, 2, 3});

			items[1] = 4; // mutate original array

			sut.Should().BeEquivalentTo(new[] {1, 2, 3, 1, 2, 3});
		}

		[TestMethod]
		public void ImmutableList_IndexOf()
		{
			var items = new[] { 1, 2, 3, 3, 3, 3 };
			var sut = new ImmutableList<int>(items);

			sut.IndexOf(8).Should().Be(-1);
			sut.IndexOf(3).Should().Be(2);
			sut.IndexOf(3, index:1, count:5, equalityComparer:null).Should().Be(2);

			((Action) (() => sut.IndexOf(8, index: 1, count: 100, equalityComparer: null))).Should().Throw<IndexOutOfRangeException>();
		}

		[TestMethod]
		public void ImmutableList_LastIndexOf()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			sut.IndexOf(8).Should().Be(-1);
			sut.IndexOf(3).Should().Be(2);
			sut.LastIndexOf(3, index: 1, count: 5, equalityComparer: null).Should().Be(5);

			((Action)(() => sut.LastIndexOf(8, index: 1, count: 100, equalityComparer: null))).Should().Throw<IndexOutOfRangeException>();
		}

		[TestMethod]
		public void ImmutableList_Add()
		{
			var sut = new ImmutableList<int>();

			var result = sut.Add(12);
			sut.Should().BeEquivalentTo(new int[] { });
			result.Single().Should().Be(12);
		}

		[TestMethod]
		public void ImmutableList_Add2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Add(12);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3, 12 });
		}

		[TestMethod]
		public void ImmutableList_Insert()
		{
			var sut = new ImmutableList<int>();

			var result = sut.Insert(0, 12);
			sut.Should().BeEquivalentTo(new int[] { });
			result.Single().Should().Be(12);
		}

		[TestMethod]
		public void ImmutableList_Insert2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Insert(4, 12);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 12, 2, 3 });
		}

		[TestMethod]
		public void InsertRange()
		{
			var sut = new ImmutableList<int>();

			var result = sut.InsertRange(0, new[] {1, 2});
			sut.Should().BeEquivalentTo(new int[] { });
			result.Should().BeEquivalentTo(new[] { 1, 2 });
		}

		[TestMethod]
		public void ImmutableList_InsertRange2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.InsertRange(4, new[] { 15, 15 });
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 15, 15, 2, 3 });
		}

		[TestMethod]
		public void ImmutableList_InsertRange3()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.InsertRange(6, new[] { 15, 15 });
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3, 15, 15 });
		}

		[TestMethod]
		public void ImmutableList_Replace()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Replace(2, 4, null);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 4, 3, 1, 4, 3 });
		}

		[TestMethod]
		public void ImmutableList_Replace2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Replace(4, 5, null);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeSameAs(sut);
		}

		[TestMethod]
		public void ImmutableList_Remove()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Remove(2, null);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 3, 1, 3 });
		}

		[TestMethod]
		public void ImmutableList_RemoveAfterAdd()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Add(12).Remove(12);
			sut.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().BeEquivalentTo(new[] { 1, 2, 3, 1, 2, 3 });
			result.Should().NotBeSameAs(sut);
		}
	}
}
