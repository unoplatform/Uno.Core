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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Core.Tests
{
	[TestClass]
	public class Given_Predicates
    {
		[TestMethod]
		public void When_True()
		{
			Assert.IsTrue(Predicates.True(true));
			Assert.IsTrue(Predicates.True(false));
			Assert.IsTrue(Predicates.True(null));
			Assert.IsTrue(Predicates.True(new object()));
			Assert.IsTrue(Predicates.True(42));

			Assert.IsTrue(Predicates<bool>.True(true));
			Assert.IsTrue(Predicates<bool>.True(false));
			Assert.IsTrue(Predicates<object>.True(null));
			Assert.IsTrue(Predicates<object>.True(new object()));
			Assert.IsTrue(Predicates<int>.True(42));
		}

		[TestMethod]
		public void When_False()
		{
			Assert.IsFalse(Predicates.False(true));
			Assert.IsFalse(Predicates.False(false));
			Assert.IsFalse(Predicates.False(null));
			Assert.IsFalse(Predicates.False(new object()));
			Assert.IsFalse(Predicates.False(42));

			Assert.IsFalse(Predicates<bool>.False(true));
			Assert.IsFalse(Predicates<bool>.False(false));
			Assert.IsFalse(Predicates<object>.False(null));
			Assert.IsFalse(Predicates<object>.False(new object()));
			Assert.IsFalse(Predicates<int>.False(42));
		}
	}
}
