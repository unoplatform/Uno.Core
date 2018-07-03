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
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Uno.Threading;

namespace Uno.Core.Tests.TestUtils
{
	internal static class SchedulerExtensions
	{
		/// <summary>
		/// Awaits a task execution on the specified scheduler, providing the result.
		/// </summary>
		/// <returns>A task that will provide the result of the execution.</returns>
		public static Task<T> Run<T>(this IScheduler source, Func<CancellationToken, Task<T>> taskBuilder, CancellationToken cancellationToken)
		{
			var completion = new FastTaskCompletionSource<T>();

			var disposable = new SingleAssignmentDisposable();
			var ctr = default(CancellationTokenRegistration);

			if (cancellationToken.CanBeCanceled)
			{
				ctr = cancellationToken.Register(() =>
				{
					completion.TrySetCanceled();
					disposable.Dispose();
				});
			}

			disposable.Disposable = source.Schedule(
				async () =>
				{
					try
					{
						var result = await taskBuilder(cancellationToken);
						completion.TrySetResult(result);
					}
					catch (Exception e)
					{
						completion.TrySetException(e);
					}
					finally
					{
						ctr.Dispose();
					}
				}
			);

			return completion.Task;
		}

		/// <summary>
		/// Awaits a task on the specified scheduler, without providing a result.
		/// </summary>
		/// <returns>A task that will complete when the work has completed.</returns>
		public static Task Run(this IScheduler source, Func<CancellationToken, Task> taskBuilder, CancellationToken ct)
		{
			return Run(
				source,
				async ct2 => { await taskBuilder(ct2); return Unit.Default; },
				ct
			);
		}
	}
}