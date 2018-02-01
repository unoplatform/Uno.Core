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
	public partial class TransactionalFixture
	{
		[TestMethod]
		public void When_UpdateObject_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new object();
			var value2 = new object();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, o =>
			{
				invocation++;
				obj = value2;
				return o;
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		[TestMethod]
		public void When_Update_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new MyClass();
			var value2 = new MyClass();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, o =>
			{
				invocation++;
				obj = value2;
				return o;
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		[TestMethod]
		public void When_Update_WithOneParameter_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new MyClass();
			var value2 = new MyClass();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, 1, (o, _) =>
			{
				invocation++;
				obj = value2;
				return o;
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		[TestMethod]
		public void When_Update_WithTwoParameter_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new MyClass();
			var value2 = new MyClass();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, 1, 2, (o, _, __) =>
			{
				invocation++;
				obj = value2;
				return o;
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		[TestMethod]
		public void When_Update_WithProjection_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new MyClass();
			var value2 = new MyClass();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, o =>
			{
				invocation++;
				obj = value2;
				return Tuple.Create(o, new object());
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		[TestMethod]
		public void When_Update_WithProjectionAndOneParameter_ReturnsOriginalValue_Then_FastExit()
		{
			var value1 = new MyClass();
			var value2 = new MyClass();

			var obj = value1;
			var invocation = 0;

			Transactional.Update(ref obj, 1, (o, _) =>
			{
				invocation++;
				obj = value2;
				return Tuple.Create(o, new object());
			});

			Assert.AreEqual(1, invocation);
			Assert.AreEqual(value2, obj);
		}

		private class MyClass
		{
		}
	}
}
