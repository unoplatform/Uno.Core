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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests
{
	[TestClass]
	public class FuncsFixture
	{
#if DEBUG
		private const TestTimeout _timeout = TestTimeout.Infinite;
#else
		private const int _timeout = 5000;
#endif

		[TestMethod]
		[Timeout(_timeout)]
		public async Task When_AsLockedMemoized_ThenSameInstance()
		{
			var mre = new ManualResetEvent(false);

			Func<object> myFunc = () => { mre.WaitOne(); return new object(); };
			var asLockedMemoized = myFunc.AsLockedMemoized();

			var a = Task.Run(asLockedMemoized);
			var b = Task.Run(asLockedMemoized);

			mre.Set();

			await Task.WhenAll(a, b);

			Assert.IsTrue(object.ReferenceEquals(a.Result, b.Result));
		}

		[TestMethod]
		[Timeout(_timeout)]
		public async Task When_AsMemoized_AndNotLocked_ThenNotSameInstance()
		{
			var started = new SemaphoreSlim(2);
			var mre = new ManualResetEvent(false);

			Func<object> myFunc = () => { mre.WaitOne(); return new object(); };
			var asMemoized = myFunc.AsMemoized();

			var a = Task.Run(() =>
			{
				started.Release();
				return asMemoized();
			});

			var b = Task.Run(() =>
			{
				started.Release();
				return asMemoized();
			});

			await started.WaitAsync();
			await started.WaitAsync();
			mre.Set();
			mre.Set();

			await Task.WhenAll(a, b);

			Assert.IsFalse(object.ReferenceEquals(a.Result, b.Result));
		}
	}
}