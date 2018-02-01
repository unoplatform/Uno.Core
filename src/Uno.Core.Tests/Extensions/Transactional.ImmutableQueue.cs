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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class Transactional_ImmutableQueue
	{
		[TestMethod]
		public void SimpleEnqueues()
		{
			var queue = ImmutableQueue<string>.Empty;

			Transactional.Enqueue(ref queue, "A");
			Transactional.Enqueue(ref queue, "B");

			queue.Should().BeEquivalentTo("A", "B");
		}

		[TestMethod]
		public void SimpleEnqueuesWithFactory()
		{
			var queue = ImmutableQueue<int>.Empty;

			Transactional.Enqueue(ref queue, q => q.Count()); // 0
			Transactional.Enqueue(ref queue, q => q.Count()); // 1
			Transactional.Enqueue(ref queue, q => q.Count()); // 2
			Transactional.Enqueue(ref queue, q => q.Count()); // 3

			queue.Should().BeEquivalentTo(0, 1, 2, 3);
		}

		[TestMethod]
		public void SimpleTryDequeues()
		{
			var queue = ImmutableQueue<int>.Empty
				.Enqueue(1)
				.Enqueue(2)
				.Enqueue(3);

			int out1;
			var r1 = Transactional.TryDequeue(ref queue, out out1);
			r1.Should().BeTrue(nameof(r1));
			out1.Should().Be(1);

			int out2;
			var r2 = Transactional.TryDequeue(ref queue, out out2);
			r2.Should().BeTrue(nameof(r2));
			out2.Should().Be(2);

			int out3;
			var r3 = Transactional.TryDequeue(ref queue, out out3);
			r3.Should().BeTrue(nameof(r3));
			out3.Should().Be(3);

			int out4;
			var r4 = Transactional.TryDequeue(ref queue, out out4);
			r4.Should().BeFalse(nameof(r4));
			out4.Should().Be(default(int));
		}

		[TestMethod]
		public void SimpleDequeues()
		{
			var queue = ImmutableQueue<int>.Empty
				.Enqueue(1)
				.Enqueue(2)
				.Enqueue(3);

			var r1 = Transactional.Dequeue<ImmutableQueue<int>, int>(ref queue);
			r1.Should().Be(1);
			var r2 = Transactional.Dequeue<ImmutableQueue<int>, int>(ref queue);
			r2.Should().Be(2);
			var r3 = Transactional.Dequeue<ImmutableQueue<int>, int>(ref queue);
			r3.Should().Be(3);

			Action r4 = () => Transactional.Dequeue<ImmutableQueue<int>, int>(ref queue);
			r4.ShouldThrow<InvalidOperationException>();
		}
	}
}
