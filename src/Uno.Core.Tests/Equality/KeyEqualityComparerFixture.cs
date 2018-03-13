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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Equality;

namespace Uno.Core.Tests.Equality
{
	[TestClass]
	public class KeyEqualityComparerFixture
	{
		private class MyEntity : IKeyEquatable<MyEntity>, IKeyEquatable
		{
			public string Key { get; }

			public string Value { get; }

			public MyEntity(string key, string value)
			{
				Key = key;
				Value = value;
			}

			public bool KeyEquals(MyEntity other) => Key.Equals(other?.Key);

			public int GetKeyHashCode() => Key?.GetHashCode() ?? 0;

			public bool KeyEquals(object other) => KeyEquals(other as MyEntity);
		}

		[TestMethod]
		public void TestKeyEqualityComparer()
		{
			var sut = KeyEqualityComparer<MyEntity>.Default;

			var a1 = new MyEntity("a", "v1");
			var a2 = new MyEntity("a", "v2");
			var b1 = new MyEntity("b", "v1");

			sut.Equals(a1, a2).Should().Be(true);
			sut.Equals(a2, a1).Should().Be(true);
			sut.Equals(a1, a1).Should().Be(true);
			sut.Equals(a2, a2).Should().Be(true);
			sut.Equals(null, a2).Should().Be(false);
			sut.Equals(a1, null).Should().Be(false);
			sut.Equals(null, null).Should().Be(true);
			sut.Equals(b1, a2).Should().Be(false);
			sut.Equals(a1, b1).Should().Be(false);
		}

		[TestMethod]
		public void TestKeyEquatableEquals()
		{
			var sut = KeyEqualityComparer.Default;

			var a1 = new MyEntity("a", "v1") as IKeyEquatable;
			var a2 = new MyEntity("a", "v2") as IKeyEquatable;
			var b1 = new MyEntity("b", "v1") as IKeyEquatable;

			a1.KeyEquals(a2).Should().Be(true);
			a2.KeyEquals(a1).Should().Be(true);
			a1.KeyEquals(a1).Should().Be(true);
			a2.KeyEquals(a2).Should().Be(true);
			a1.KeyEquals(null).Should().Be(false);
			a1.KeyEquals("!!").Should().Be(false);
			b1.KeyEquals(a2).Should().Be(false);
			a1.KeyEquals(b1).Should().Be(false);

			sut.Equals(a1, a2).Should().Be(true);
			sut.Equals(a2, a1).Should().Be(true);
			sut.Equals(a1, a1).Should().Be(true);
			sut.Equals(a2, a2).Should().Be(true);
			sut.Equals(null, a2).Should().Be(false);
			sut.Equals(a1, null).Should().Be(false);
			sut.Equals(null, null).Should().Be(true);
			sut.Equals(b1, a2).Should().Be(false);
			sut.Equals(a1, b1).Should().Be(false);
		}
	}
}
