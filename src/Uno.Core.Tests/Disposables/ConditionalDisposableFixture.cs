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
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Disposables;
using Uno.Extensions;

namespace Uno.Core.Tests
{
	[TestClass]
	public class ConditionalDisposableFixture
	{
		[TestMethod]
		public async Task When_Dispose_And_TargetAlive()
		{
			var target = new object();
			bool disposed = false;

			var SUT = new ConditionalDisposable(target, () => disposed = true);

			Assert.IsFalse(disposed);

			SUT.Dispose();

			Assert.IsTrue(disposed);
		}

		[TestMethod]
		public async Task When_TargetCollected_And_Dispose()
		{
			bool disposed = false;

			(ConditionalDisposable, WeakReference) Build()
			{
				var target = new object();
				var weakTargetInner = new WeakReference(target);

				var SUTinner = new ConditionalDisposable(target, () => disposed = true);

				return (SUTinner, weakTargetInner);
			}

			var (SUT, weakTarget) = Build();

			await Wait(() => weakTarget.IsAlive);

			Assert.IsFalse(disposed);

			SUT.Dispose();

			Assert.IsTrue(disposed);
		}

		[TestMethod]
		public async Task When_Source_Disposed_And_Dereferenced()
		{
			bool disposed = false;

			(WeakReference<ConditionalDisposable>, object) Build()
			{
				var target = new object();

				var SUTinner = new ConditionalDisposable(target, () => disposed = true);
				SUTinner.Dispose();

				return (new WeakReference<ConditionalDisposable>(SUTinner), target);
			}

			var (SUT, weakTarget) = Build();

			await Wait(() => SUT.IsAlive);

			Assert.IsTrue(disposed);
		}

		[TestMethod]
		public async Task When_Source_And_Target_Dereferenced()
		{
			bool disposed = false;

			(WeakReference<ConditionalDisposable>, WeakReference) Build()
			{
				var target = new object();

				var SUTinner = new ConditionalDisposable(target, () => disposed = true);

				return (new WeakReference<ConditionalDisposable>(SUTinner), new WeakReference(target));
			}

			var (weakSUT, weakTarget) = Build();

			await Wait(() => weakSUT.IsAlive || weakTarget.IsAlive);

			Assert.IsTrue(disposed);
		}

		async Task Wait(Func<bool> predicate, TimeSpan? timeout = null)
		{
			timeout = timeout ?? TimeSpan.FromSeconds(2);

			var sw = Stopwatch.StartNew();
			while (predicate() && sw.Elapsed < timeout)
			{
				await Task.Delay(100);
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
	}
}