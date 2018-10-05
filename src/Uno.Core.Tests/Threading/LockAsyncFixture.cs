using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Threading;

namespace Uno.Core.Tests.Threading
{
	[TestClass]
	public class LockAsyncFixture
	{
		[TestMethod]
		public async Task BasicLock()
		{
			var gate = new AsyncLock();
			var sync = new object();
			var gate2 = new object();
			var current = 0;
			var maxCurrent = 0;
			var waiting = 0;

			var func1 = Funcs.Create(async ct => {

				await Task.Yield();

				lock (sync)
				{
					waiting++;
					Monitor.Wait(sync);
				}
			
				using(await gate.LockAsync(ct))
				{
					lock (gate2)
					{
						current++;
						maxCurrent = Math.Max(current, maxCurrent);
					}

					await Task.Delay(100);

					lock (gate2)
					{
						current--;
					}
				}

				return 42;
			});

			var c1 = func1(CancellationToken.None);
			var c2 = func1(CancellationToken.None);

			while (waiting != 2)
			{
				await Task.Delay(100);
			}

			lock(sync)
			{
				Monitor.PulseAll(sync);
			}

			await c1;
			await c2;

			Assert.AreEqual(1, maxCurrent);
		}

		[TestMethod]
		public async Task CancelledLock()
		{
			var gate = new AsyncLock();

			var func = Funcs.Create(async (int wait, CancellationToken ct) => {
			
				using(await gate.LockAsync(ct))
				{
					await Task.Delay(wait, ct);
				}

				return 42;
			});

			using (await gate.LockAsync(CancellationToken.None))
			{
				var cts = new CancellationTokenSource();
				var r1 = func(100000, cts.Token);

				cts.Cancel();

				Assert.AreEqual(TaskStatus.Canceled, r1.Status);
			}
			
			var r2 = await func(1000, CancellationToken.None);
			Assert.AreEqual(42, r2);
		}
	}
}
