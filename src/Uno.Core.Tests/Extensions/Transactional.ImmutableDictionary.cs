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
using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class Transactional_ImmutableDictionary
	{
		[TestMethod]
		public void SimpleGetOrAdd()
		{
			var dict = ImmutableDictionary<string, int>
				.Empty
				.Add("A", 11)
				.Add("B", 22)
				.Add("C", 33);

			var r1 = Transactional.GetOrAdd(ref dict, "A", x => x.Length);
			r1.Should().Be(11);
			dict.Should().HaveCount(3);

			var r2 = Transactional.GetOrAdd(ref dict, "D", x => 44);
			r2.Should().Be(44);
			dict.Should().HaveCount(4);
		}
	}
}
