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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class FuncExtensions_ApplyMemoize
	{
		int MyMethodCalled = 0;

		[TestMethod]
		public void ApplyMemoizeNoParam()
		{
			MyMethodCalled = 0;

			var i1 = new object();
			var a1 = i1.ApplyMemoized(MyMethod);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethod);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			i1 = new object();
			GC.Collect();

			a1 = i1.ApplyMemoized(MyMethod);

			Assert.AreEqual(i1, a1);
			Assert.AreEqual(2, MyMethodCalled);
		}

		[TestMethod]
		public void ApplyMemoizeOneParam()
		{
			MyMethodCalled = 0;

			var i1 = new object();
			var a1 = i1.ApplyMemoized(MyMethodOneParam, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodOneParam, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodOneParam, 2);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(2, MyMethodCalled);

			i1 = new object();
			GC.Collect();

			a1 = i1.ApplyMemoized(MyMethodOneParam, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(3, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodOneParam, 2);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(4, MyMethodCalled);
		}

		[TestMethod]
		public void ApplyMemoizeTwoParam()
		{
			MyMethodCalled = 0;

			var i1 = new object();
			var a1 = i1.ApplyMemoized(MyMethodTwoParam, 1, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodTwoParam, 1, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(1, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodTwoParam, 2, 2);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(2, MyMethodCalled);

			i1 = new object();
			GC.Collect();

			a1 = i1.ApplyMemoized(MyMethodTwoParam, 1, 1);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(3, MyMethodCalled);

			a1 = i1.ApplyMemoized(MyMethodTwoParam, 2, 2);
			Assert.AreEqual(i1, a1);
			Assert.AreEqual(4, MyMethodCalled);
		}

		public object MyMethod(object instance)
		{
			MyMethodCalled++;
			return instance;
		}

		public object MyMethodOneParam(object instance, int value)
		{
			MyMethodCalled++;
			return instance;
		}

		public object MyMethodTwoParam(object instance, int value, int value2)
		{
			MyMethodCalled++;
			return instance;
		}
	}
}
