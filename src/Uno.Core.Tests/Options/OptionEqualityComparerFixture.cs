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

namespace Uno.Core.Tests.Options
{
	[TestClass]
	public class OptionEqualityComparerFixture
	{
		[TestMethod]
		public void TestOptionComparerWithNoneValues()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.None<string>();
			var b = Option.None<string>();

			sut.Equals(a, b).Should().BeTrue();
		}

		[TestMethod]
		public void TestOptionComparerWithNoneAndNullValues()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.None<string>();

			sut.Equals(a, null).Should().BeTrue();
		}

		[TestMethod]
		public void TestOptionComparerWithNoneAndNullValues2()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.None<string>();

			sut.Equals(null, a).Should().BeTrue();
		}

		[TestMethod]
		public void TestOptionComparerWithNoneAndSomeNullValues()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.None<string>();
			var b = Option.Some<string>(null);

			sut.Equals(a, b).Should().BeFalse();
		}

		[TestMethod]
		public void TestOptionComparerWithNullAndSomeNullValues()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.Some<string>(null);

			sut.Equals(a, null).Should().BeFalse();
		}

		[TestMethod]
		public void TestOptionComparerWithSomeValues_WhenEqual()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.Some<string>("a");

			sut.Equals(a, "a").Should().BeTrue(); // implicit casting is wanted here
		}

		[TestMethod]
		public void TestOptionComparerWithSomeNullValues_WhenEqual()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.Some<string>(null);
			var b = Option.Some<string>(null);

			sut.Equals(a, b).Should().BeTrue();
		}

		[TestMethod]
		public void TestOptionComparerWithSomeValues_WhenNotEqual()
		{
			var sut = new OptionEqualityComparer<string>();

			var a = Option.Some<string>("a");
			var b = Option.Some<string>("b");

			sut.Equals(a, b).Should().BeFalse();
		}
	}
}
