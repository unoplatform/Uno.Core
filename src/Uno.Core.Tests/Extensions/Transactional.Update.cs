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
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class Transactional_Update
	{
		[TestMethod]
		public async Task SimpleUpdate()
		{
			var list = ImmutableList<int>.Empty;

			Transactional.Update(ref list, l => l.Add(42));

			list.Should().HaveCount(1);
			list[0].Should().Be(42);
		}

		[TestMethod]
		public async Task SimpleUpdateWith1Param()
		{
			var list = ImmutableList<int>.Empty;

			Transactional.Update(ref list, 42, (l, x) => l.Add(x));

			list.Should().HaveCount(1);
			list[0].Should().Be(42);
		}

		[TestMethod]
		public async Task SimpleUpdateWith2Params()
		{
			var list = ImmutableList<int>.Empty;

			Transactional.Update(ref list, 42, 10000, (l, x, y) => l.Add(y - x));

			list.Should().HaveCount(1);
			list[0].Should().Be(9958);
		}

		[TestMethod]
		public async Task SimpleUpdateWithTuple()
		{
			var list = ImmutableList<int>.Empty;

			var res = Transactional.Update(ref list, l => Tuple.Create(l.Add(42), 21));

			list.Should().HaveCount(1);
			res.Should().Be(21);
		}

		[TestMethod]
		public async Task AtomicUpdate()
		{
			var list = ImmutableList<int>.Empty;

			var ev = new AutoResetEvent(false);

			var runs1 = 0;

			var t = Task.Run(() =>
			{
				Transactional.Update(ref list, l =>
				{
					var ret = l.Add(42);

					// We've capture the list, release the thread that will create
					// the race condition
					ev.Set();

					// Now wait for the race to happen
					ev.WaitOne();
					runs1++;

					return ret;
				});
			});

			// Wait for the other thread to have captured the list
			ev.WaitOne();

			// Update the list reference while the other thread is updaint 
			Transactional.Update(ref list, l => l.Add(42));

			// Release the waiting thread.
			ev.Set();

			await t;

			list.Should().HaveCount(2);
			runs1.Should().Be(2);
		}
	}
}
