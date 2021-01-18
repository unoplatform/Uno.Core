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
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests.Collections
{
	[TestClass]
	public class ListExtensionsFixture
	{
		private static readonly int[] list = new [] { 1, 2, 3};

		[TestMethod]
		public void AsReadOnly()
		{
			IList<int> list2 = new List<int>(list);

			Assert.IsTrue(list2.AsReadOnly().IsReadOnly);
		}

		[TestMethod]
		public void Adapt()
		{
			IList<string> adapter = list.Adapt<int, string>();

			Assert.IsNotNull(adapter);
		}

		[TestMethod]
		public void IndexOf_WithComparer()
		{
			var @true = new MyComparer((l, r) => true);
			var @false = new MyComparer((l, r) => false);

			Assert.AreEqual(0, new[] {1, 2, 3}.IndexOf(2, @true));
			Assert.AreEqual(1, new[] {1, 2, 3}.IndexOf(2, EqualityComparer<object>.Default));
			Assert.AreEqual(1, new[] {1, 2, 3}.IndexOf(2, default(IEqualityComparer)));
			Assert.AreEqual(-1, new[] {1, 2, 3}.IndexOf(2, @false));

			Assert.AreEqual(0, new[] {1, 2, 3}.IndexOf(-1, @true));
			Assert.AreEqual(-1, new[] {1, 2, 3}.IndexOf(-1, EqualityComparer<object>.Default));
			Assert.AreEqual(-1, new[] {1, 2, 3}.IndexOf(-1, default(IEqualityComparer)));
			Assert.AreEqual(-1, new[] {1, 2, 3}.IndexOf(-1, @false));
		}

		private class MyComparer : IEqualityComparer
		{
			private readonly Func<object, object, bool> _equals;

			public MyComparer(Func<object, object, bool> equals) => _equals = @equals;

			public bool Equals(object left, object right) => _equals(left, right);

			public int GetHashCode(object obj) => throw new System.NotImplementedException();
		}

		[TestMethod]
		public void When_FluentAdd_Then_InferTypeProperly()
		{
			new MyList().FluentAdd(new MyItem());
		}

		[TestMethod]
		public void When_FluentAdd_Then_Add()
		{
			new MyList().FluentAdd(new MyItem()).Should().HaveCount(1);
		}

		[TestMethod]
		public void When_FluentAdd_Then_Fluent()
		{
			var c1 = new MyList();
			var c2 = c1.FluentAdd(new MyItem());

			c1.Should().BeSameAs(c2);
		}

		private class MyList : IList
		{
			private IList _inner;

			/// <inheritdoc />
			public IEnumerator GetEnumerator() => _inner.GetEnumerator();

			/// <inheritdoc />
			public void CopyTo(Array array, int index)
			{
				_inner.CopyTo(array, index);
			}

			/// <inheritdoc />
			public int Count => _inner.Count;

			/// <inheritdoc />
			public object SyncRoot => _inner.SyncRoot;

			/// <inheritdoc />
			public bool IsSynchronized => _inner.IsSynchronized;

			/// <inheritdoc />
			public int Add(object value) => _inner.Add(value);

			/// <inheritdoc />
			public bool Contains(object value) => _inner.Contains(value);

			/// <inheritdoc />
			public void Clear()
			{
				_inner.Clear();
			}

			/// <inheritdoc />
			public int IndexOf(object value) => _inner.IndexOf(value);

			/// <inheritdoc />
			public void Insert(int index, object value)
			{
				_inner.Insert(index, value);
			}

			/// <inheritdoc />
			public void Remove(object value)
			{
				_inner.Remove(value);
			}

			/// <inheritdoc />
			public void RemoveAt(int index)
			{
				_inner.RemoveAt(index);
			}

			/// <inheritdoc />
			public object this[int index]
			{
				get => _inner[index];
				set => _inner[index] = value;
			}

			/// <inheritdoc />
			public bool IsReadOnly => _inner.IsReadOnly;

			/// <inheritdoc />
			public bool IsFixedSize => _inner.IsFixedSize;
		}

		private class MyItem
		{
		}
	}
}
