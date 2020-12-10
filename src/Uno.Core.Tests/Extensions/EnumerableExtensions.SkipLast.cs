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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests.Extensions
{
	[TestClass]
	public class EnumerableExtensions
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SkipLast_NullCheck() => default(ICollection<object>).SkipLast(5);

		[TestMethod]
		public void SkipLast_When_Skip_Negative_Then_GoesThrough()
		{
			var source = Enumerable.Range(0, 42);

			var sut = source.SkipLast(-1);

			Assert.AreSame(source, sut);
		}

		[TestMethod]
		public void SkipLast_When_Skip_0_Then_GoesThrough()
		{
			var source = Enumerable.Range(0, 42);

			var sut = source.SkipLast(0);

			Assert.AreSame(source, sut);
		}

		[TestMethod]
		public void SkipLast_When_Skip_1()
		{
			var source = Enumerable.Range(0, 42);

			var sut = source.SkipLast(1);
			
			CollectionAssert.AreEqual(source.Take(42 - 1).ToArray(), sut.ToArray());
		}

		[TestMethod]
		public void SkipLast_When_Skip_N()
		{
			var source = Enumerable.Range(0, 42);

			var sut = source.SkipLast(5);

			CollectionAssert.AreEqual(source.Take(42 - 5).ToArray(), sut.ToArray());
		}


		[TestMethod]
		public void SkipLast_When_InfiniteEnumerable()
		{
			var source = Infinite();

			var sut = source.SkipLast(5);

			sut.Take(15).ToArray();
		}

		private IEnumerable<object> Infinite()
		{
			var value = new object();
			while (true)
			{
				yield return value;
			}
		}
	}
}
