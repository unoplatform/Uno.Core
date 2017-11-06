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

			sut.SequenceEqual(new[] {1, 2, 3, 1, 2, 3}).ShouldBeEquivalentTo(true);

			items[1] = 4; // mutate original array

			sut.SequenceEqual(new[] {1, 2, 3, 1, 2, 3}).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void ImmutableList_IndexOf()
		{
			var items = new[] { 1, 2, 3, 3, 3, 3 };
			var sut = new ImmutableList<int>(items);

			sut.IndexOf(8).ShouldBeEquivalentTo(-1);
			sut.IndexOf(3).ShouldBeEquivalentTo(2);
			sut.IndexOf(3, index:1, count:5, equalityComparer:null).ShouldBeEquivalentTo(2);

			((Action) (() => sut.IndexOf(8, index: 1, count: 100, equalityComparer: null))).ShouldThrow<IndexOutOfRangeException>();
		}

		[TestMethod]
		public void ImmutableList_LastIndexOf()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			sut.IndexOf(8).ShouldBeEquivalentTo(-1);
			sut.IndexOf(3).ShouldBeEquivalentTo(2);
			sut.LastIndexOf(3, index: 1, count: 5, equalityComparer: null).ShouldBeEquivalentTo(5);

			((Action)(() => sut.LastIndexOf(8, index: 1, count: 100, equalityComparer: null))).ShouldThrow<IndexOutOfRangeException>();
		}

		[TestMethod]
		public void ImmutableList_Add()
		{
			var sut = new ImmutableList<int>();

			var result = sut.Add(12);
			sut.SequenceEqual(new int[] { }).ShouldBeEquivalentTo(true);
			result.Single().ShouldBeEquivalentTo(12);
		}

		[TestMethod]
		public void ImmutableList_Add2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Add(12);
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3, 12 }).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void ImmutableList_Insert()
		{
			var sut = new ImmutableList<int>();

			var result = sut.Insert(0, 12);
			sut.SequenceEqual(new int[] { }).ShouldBeEquivalentTo(true);
			result.Single().ShouldBeEquivalentTo(12);
		}

		[TestMethod]
		public void ImmutableList_Insert2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Insert(4, 12);
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2, 3, 1, 12, 2, 3 }).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void InsertRange()
		{
			var sut = new ImmutableList<int>();

			var result = sut.InsertRange(0, new[] {1, 2});
			sut.SequenceEqual(new int[] { }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2 }).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void ImmutableList_InsertRange2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.InsertRange(4, new[] { 15, 15 });
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2, 3, 1, 15, 15, 2, 3 }).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void ImmutableList_InsertRange3()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.InsertRange(6, new[] { 15, 15 });
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3, 15, 15 }).ShouldBeEquivalentTo(true);
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
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 3, 1, 3 }).ShouldBeEquivalentTo(true);
		}

		[TestMethod]
		public void ImmutableList_Remove2()
		{
			var items = new[] { 1, 2, 3, 1, 2, 3 };
			var sut = new ImmutableList<int>(items);

			var result = sut.Add(12);
			sut.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3 }).ShouldBeEquivalentTo(true);
			result.SequenceEqual(new[] { 1, 2, 3, 1, 2, 3, 12 }).ShouldBeEquivalentTo(true);
		}
	}
}
