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

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions.ValueType;

namespace Uno.Tests.Extensions.ValueType
{
	[TestClass]
	public class ValueSupportFixture
	{
		[TestMethod]
		public void Enum_Add()
		{
			var flags = BindingFlags.Public;
			flags = flags.Add(BindingFlags.NonPublic);

			Assert.AreEqual(BindingFlags.Public | BindingFlags.NonPublic, flags);
		}

		[TestMethod]
		public void Enum_Remove()
		{
			var flags = BindingFlags.Public | BindingFlags.NonPublic;
			flags = flags.Substract(BindingFlags.NonPublic);

			Assert.AreEqual(BindingFlags.Public, flags);
		}

		[TestMethod]
		public void Enum_ContainsAll()
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;

			Assert.IsTrue(flags.ContainsAll(BindingFlags.Public));
			Assert.IsTrue(flags.ContainsAll(BindingFlags.NonPublic));
			Assert.IsTrue(flags.ContainsAll(BindingFlags.Public | BindingFlags.NonPublic));

			Assert.IsFalse(flags.ContainsAll(BindingFlags.Public | BindingFlags.Instance));
			Assert.IsFalse(flags.ContainsAll(BindingFlags.Instance));

		}

		[TestMethod]
		public void Enum_ContainsAny()
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;

			Assert.IsTrue(flags.ContainsAny(BindingFlags.Public));
			Assert.IsTrue(flags.ContainsAny(BindingFlags.NonPublic));
			Assert.IsTrue(flags.ContainsAny(BindingFlags.Public | BindingFlags.NonPublic));
			Assert.IsTrue(flags.ContainsAny(BindingFlags.Public | BindingFlags.Instance));

			Assert.IsFalse(flags.ContainsAny(BindingFlags.Instance));
		}
	}
}
