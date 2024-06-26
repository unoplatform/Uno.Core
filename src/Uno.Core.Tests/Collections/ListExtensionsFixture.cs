﻿// ******************************************************************
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Core.Collections;
using Uno.Extensions;

namespace Uno.Core.Tests.Collections
{
	[TestClass]
	public class ListExtensionsFixture
	{
		private static readonly int[] list = new [] { 1, 2, 3};

#if !NET7_0_OR_GREATER
		[TestMethod]
		public void AsReadOnly()
		{
			IList<int> list2 = new List<int>(list);

			Assert.IsTrue(list2.AsReadOnly().IsReadOnly);
		}
#endif

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
	}
}
