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
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Core.Tests
{
	[TestClass]
	public class DelegateConditionalWeakTableFixture
	{
		#region _ Delegates_ 
		private static string StaticMethod(int v) => v.ToString();

		private string Method(int v) => v.ToString();

		public Func<int, string> Create()
		{
			this.IsNotStatic();

			return i => i.ToString();
		}

		public Func<int, string> CreateWithCapture()
		{
			this.IsNotStatic();

			var a = new Random().Next();
			return i => a.ToString();
		}

		private void IsNotStatic()
		{
			var _ = this.ToString();
			if (_.Length == 0)
			{
			}
		} 
		#endregion

		#region Validate that the behavior of delegates did not change
		[TestMethod]
		public void _DelegateBehavior_When_StaticMethod()
		{
			Func<int, string> delegate1 = StaticMethod;
			Func<int, string> delegate2 = StaticMethod;

			Assert.AreNotSame(delegate1, delegate2);
			Assert.AreEqual(delegate1, delegate2);
			Assert.AreEqual(delegate1.Target, delegate2.Target);
			Assert.AreEqual(delegate1.Method, delegate2.Method);
		}

		[TestMethod]
		public void _DelegateBehavior_When_Method()
		{
			Func<int, string> delegate1 = Method;
			Func<int, string> delegate2 = Method;

			Assert.AreNotSame(delegate1, delegate2);
			Assert.AreEqual(delegate1, delegate2);
			Assert.AreEqual(delegate1.Target, delegate2.Target);
			Assert.AreEqual(delegate1.Method, delegate2.Method);
		}

		[TestMethod]
		public void _DelegateBehavior_When_Lamba()
		{
			Func<int, string> delegate1 = i => i.ToString();
			Func<int, string> delegate2 = i => i.ToString();

			Assert.AreNotSame(delegate1, delegate2);
			Assert.AreNotEqual(delegate1, delegate2);
			Assert.AreEqual(delegate1.Target, delegate2.Target);
			Assert.AreNotEqual(delegate1.Method, delegate2.Method);
		}

		[TestMethod]
		public void _DelegateBehavior_When_ExternalLamba()
		{
			Func<int, string> delegate1 = Create();
			Func<int, string> delegate2 = Create();

			Assert.AreSame(delegate1, delegate2);
			Assert.AreEqual(delegate1, delegate2);
			Assert.AreEqual(delegate1.Target, delegate2.Target);
			Assert.AreEqual(delegate1.Method, delegate2.Method);
		}

		[TestMethod]
		public void _DelegateBehavior_When_Lamba_WithCapture()
		{
			var a = new Random().Next();

			Func<int, string> delegate1 = i => a.ToString();
			Func<int, string> delegate2 = i => a.ToString();

			Assert.AreNotSame(delegate1, delegate2);
			Assert.AreNotEqual(delegate1, delegate2);
			Assert.AreEqual(delegate1.Target, delegate2.Target);
			Assert.AreNotEqual(delegate1.Method, delegate2.Method);
		}

		[TestMethod]
		public void _DelegateBehavior_When_ExternalLamba_WithCapture()
		{
			Func<int, string> delegate1 = CreateWithCapture();
			Func<int, string> delegate2 = CreateWithCapture();

			Assert.AreNotSame(delegate1, delegate2);
			Assert.AreNotEqual(delegate1, delegate2);
			Assert.AreNotEqual(delegate1.Target, delegate2.Target);
			Assert.AreEqual(delegate1.Method, delegate2.Method);
		} 
		#endregion

		[TestMethod]
		public void When__StaticMethod()
		{
			Func<int, string> delegate1 = StaticMethod;
			Func<int, string> delegate2 = StaticMethod;

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreSame(value1, value2);
		}

		[TestMethod]
		public void When__Method()
		{
			Func<int, string> delegate1 = Method;
			Func<int, string> delegate2 = Method;

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreSame(value1, value2);
		}

		[TestMethod]
		public void When__Lamba()
		{
			Func<int, string> delegate1 = i => i.ToString();
			Func<int, string> delegate2 = i => i.ToString();

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreNotSame(value1, value2); // That sucks
		}

		[TestMethod]
		public void When__ExternalLamba()
		{
			Func<int, string> delegate1 = Create();
			Func<int, string> delegate2 = Create();

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreSame(value1, value2);
		}

		[TestMethod]
		public void When__Lamba_WithCapture()
		{
			var a = new Random().Next();

			Func<int, string> delegate1 = i => a.ToString();
			Func<int, string> delegate2 = i => a.ToString();

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreNotSame(value1, value2);
		}

		[TestMethod]
		public void When__ExternalLamba_WithCapture()
		{
			Func<int, string> delegate1 = CreateWithCapture();
			Func<int, string> delegate2 = CreateWithCapture();

			var sut = new DelegateConditionalWeakTable<object>();

			var value1 = sut.GetValue(delegate1, _ => new object());
			var value2 = sut.GetValue(delegate2, _ => new object());

			Assert.AreNotSame(value1, value2);
		}
	}
}
