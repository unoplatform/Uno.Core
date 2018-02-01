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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class ObjectExtensionsFixture
	{
		[TestMethod]
		public void Extensions()
		{
			string s = "A";

			Assert.AreEqual(s, s.Extensions().ExtendedValue);
		}

		[TestMethod]
		public void IsDefault()
		{
			Assert.IsTrue(0.Extensions().IsDefault());
			Assert.IsFalse(1.Extensions().IsDefault());

			Assert.IsTrue(((string)null).Extensions().IsDefault());
			Assert.IsFalse("A".Extensions().IsDefault());
		}

		[TestMethod]
		public void Dispose_Disposable()
		{
			bool called = false;

			object disposable = Actions.Create(() => called = true).ToDisposable();

			disposable.Extensions().Dispose();

			Assert.IsTrue(called);
		}

		[TestMethod]
		public void Dispose_NonDisposable()
		{
			new object().Extensions().Dispose();
		}

		[TestMethod]
		public void Using_Disposable()
		{
			object disposable = Actions.Create(() => { }).ToDisposable();

			Assert.AreSame(disposable, disposable.Extensions().Using());
		}

		[TestMethod]
		public void Using_NonDisposable()
		{
			Assert.AreSame(NullDisposable.Instance, new object().Extensions().Using());
		}
	}
}
