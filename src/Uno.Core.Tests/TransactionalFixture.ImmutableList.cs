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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Core.Tests
{
	partial class TransactionalFixture
	{
		[TestMethod]
		public void When_List_AddDistinct_Then_ItemAdded()
		{
			var list = ImmutableList<object>.Empty;
			var item = new object();

			Transactional.AddDistinct(ref list, item, EqualityComparer<object>.Default);

			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list.Contains(item));
		}

		[TestMethod]
		public void When_List_AddDistinct_Twice_Then_ItemAddedOnlyOnce()
		{
			var list = ImmutableList<object>.Empty;
			var item = new object();

			var result1 = Transactional.AddDistinct(ref list, item, EqualityComparer<object>.Default);
			var result2 = Transactional.AddDistinct(ref list, item, EqualityComparer<object>.Default);

			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list.Contains(item));
			Assert.AreSame(result1, result2);
		}
	}
}
