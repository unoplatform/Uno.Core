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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class FormattableExtensionsFixture
	{
		[TestMethod]
		public void TestSimpleByteToStringInvariant()
		{
			const byte b = 123;
			Assert.AreEqual("123", b.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleSByteToStringInvariant()
		{
			const sbyte b = -123;
			Assert.AreEqual("-123", b.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleShortToStringInvariant()
		{
			const short s = -1234;
			Assert.AreEqual("-1234", s.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleUShortToStringInvariant()
		{
			const ushort us = 1234;
			Assert.AreEqual("1234", us.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleIntToStringInvariant()
		{
			const int i = -1234;
			Assert.AreEqual("-1234", i.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleUIntToStringInvariant()
		{
			const uint ui = 1234;
			Assert.AreEqual("1234", ui.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleDoubleToStringInvariant()
		{
			const double d = -1234.5678;
			Assert.AreEqual("-1234.5678", d.ToStringInvariant());
		}

		[TestMethod]
		public void TestSimpleSingleToStringInvariant()
		{
			const float d = -1234.567F;
			Assert.AreEqual("-1234.567", d.ToStringInvariant());
		}
	}
}
