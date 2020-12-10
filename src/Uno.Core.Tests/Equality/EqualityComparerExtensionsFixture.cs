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
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Core.Equality;

namespace Uno.Core.Tests.Equality
{
	[TestClass]
	public class EqualityComparerExtensionsFixture
	{
		private class MyGenericEqualityComparerString : IEqualityComparer<string>
		{
			public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

			public int GetHashCode(string obj) => obj?.GetHashCode() ?? -100;
		}

		private class MyGenericEqualityComparerInt : IEqualityComparer<int>
		{
			public bool Equals(int x, int y) => x == y;

			public int GetHashCode(int obj) => obj | int.MaxValue;
		}

		[TestMethod]
		public void Test_GenericToNonGenericAdapter_String()
		{
			var genericComparer = new MyGenericEqualityComparerString();
			var nonGenericComparer = genericComparer.ToEqualityComparer();

			nonGenericComparer.Equals("a", 2).Should().Be(false);
			nonGenericComparer.Equals("a", "A").Should().Be(true);
			nonGenericComparer.Equals("", "").Should().Be(true);
			nonGenericComparer.Equals(2, null).Should().Be(false);
			nonGenericComparer.Equals(2, 2).Should().Be(false); // !! this is correct.
			nonGenericComparer.Equals(null, 2).Should().Be(false);
			nonGenericComparer.Equals(null, "").Should().Be(false);
			nonGenericComparer.Equals(null, null).Should().Be(true);

			nonGenericComparer.GetHashCode(null).Should().Be(-100);
		}

		[TestMethod]
		public void Test_GenericToNonGenericAdapter_Int()
		{
			var genericComparer = new MyGenericEqualityComparerInt();
			var nonGenericComparer = genericComparer.ToEqualityComparer();

			nonGenericComparer.Equals("a", 2).Should().Be(false);
			nonGenericComparer.Equals("a", "A").Should().Be(false);
			nonGenericComparer.Equals("", "").Should().Be(false);
			nonGenericComparer.Equals(2, null).Should().Be(false);
			nonGenericComparer.Equals(2, 2).Should().Be(true);
			nonGenericComparer.Equals(null, 2).Should().Be(false);
			nonGenericComparer.Equals(null, "").Should().Be(false);
			nonGenericComparer.Equals(null, null).Should().Be(false); // this is correct.

			nonGenericComparer.GetHashCode(null).Should().Be(-1);
		}

		private class MyEqualityComparerString : IEqualityComparer
		{
			bool IEqualityComparer.Equals(object x, object y)
			{
				return string.Equals(x as string, y as string, StringComparison.OrdinalIgnoreCase);
			}

			public int GetHashCode(object obj)
			{
				return (obj as string)?.GetHashCode() ?? -100;
			}
		}

		[TestMethod]
		public void Test_NonGenericTogenericAdapter_String()
		{
			var nonGericComparer = new MyEqualityComparerString();
			var sut = nonGericComparer.ToEqualityComparer<string>();

			sut.Equals("a", "A").Should().Be(true);
		}
	}
}
