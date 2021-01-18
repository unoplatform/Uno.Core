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
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nVentive.Umbrella.Concurrency;
using Uno.Core.Tests.TestUtils;
using Uno.Threading;

namespace Uno.Core.Tests.Threading
{
	[TestClass]
	public class FastTaskCompletionSourceFixture
	{
		[TestMethod]
		public void TestSetResultSynchronously1()
		{
			var sut = new TaskCompletionSource<string>();

			sut.SetResult("1234");

			// The task is created AFTER the result being set
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.RanToCompletion);
			task.Result.Should().BeEquivalentTo("1234");
		}

		[TestMethod]
		public void TestSetResultSynchronously2()
		{
			var sut = new TaskCompletionSource<string>();

			// The task is created BEFORE the result being set
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			sut.SetResult("1234");

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.RanToCompletion);
			task.Result.Should().BeEquivalentTo("1234");
		}

		[TestMethod]
		public void TestSetResultAsynchronously()
		{
			var scheduler = new TestScheduler();
			var sut = new TaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					var r = await sut.Task;
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					return r;
				}, CancellationToken.None);

			task.IsCompleted.Should().BeFalse();
			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeFalse();
			sut.SetResult("1234");

			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.RanToCompletion);
			task.Result.Should().BeEquivalentTo("1234");

			scheduler.Stop();
		}

		[TestMethod]
		public void TestSetResultAsynchronously_AndAwaitDirectly1()
		{
			var scheduler = new TestScheduler();
			var sut = new TaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					var r = await sut.Task;
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					return r;
				}, CancellationToken.None);

			task.IsCompleted.Should().BeFalse();
			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeFalse();
			sut.SetResult("1234");

			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.RanToCompletion);
			task.Result.Should().BeEquivalentTo("1234");

			scheduler.Stop();
		}

		[TestMethod]
		public void TestSetResultAsynchronously_AndAwaitDirectly2()
		{
			var scheduler = new TestScheduler();
			var sut = new TaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			sut.SetResult("1234");

			scheduler.AdvanceBy(5);

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					var r = await sut.Task;
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					return r;
				}, CancellationToken.None);

			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.RanToCompletion);
			task.Result.Should().BeEquivalentTo("1234");

			scheduler.Stop();
		}

		[TestMethod]
		public void TestSetCanceledSynchronously1()
		{
			var sut = new TaskCompletionSource<string>();

			sut.SetCanceled();

			// The task is created AFTER the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);
		}

		[TestMethod]
		public void TestSetCanceledSynchronously2()
		{
			var sut = new TaskCompletionSource<string>();

			// The task is created BEFORE the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			sut.SetCanceled();

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);
		}

		[TestMethod]
		public void TestSetCanceledAsynchronously()
		{
			var scheduler = new TestScheduler();
			var sut = new TaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					await sut.Task;
					throw new Exception("should not reach here");
				}, CancellationToken.None);

			task.IsCompleted.Should().BeFalse();
			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeFalse();

			sut.SetCanceled();

			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);

			scheduler.Stop();
		}

		[TestMethod]
		public void TestSetExceptionSynchronously1()
		{
			var sut = new TaskCompletionSource<string>();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ex);
			}

			// The task is created AFTER the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Faulted);
			task.Exception.Should().NotBeNull();
			var aggregateException = task.Exception;
			aggregateException.InnerExceptions.Should().HaveCount(1);
			var argException = aggregateException.InnerException;
			argException.Should().BeOfType<ArgumentNullException>();
			argException.TargetSite.Name.Should().BeEquivalentTo(nameof(TestSetExceptionSynchronously1), "original context is lost");

		}

		[TestMethod]
		public void TestSetExceptionSynchronously2()
		{
			var sut = new TaskCompletionSource<string>();

			// The task is created BEFORE the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ex);
			}

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Faulted);
			task.Exception.Should().NotBeNull();
			var aggregateException = task.Exception;
			aggregateException.InnerExceptions.Should().HaveCount(1);
			var argException = aggregateException.InnerException;
			argException.Should().BeOfType<ArgumentNullException>();
			argException.TargetSite.Name.Should().BeEquivalentTo(nameof(TestSetExceptionSynchronously2), "original context is lost");
		}

		[TestMethod]
		public void TestSetExceptionAsynchronously()
		{
			var scheduler = new TestScheduler();
			var sut = new TaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					await sut.Task;
					throw new Exception("should not reach here");
				}, CancellationToken.None);

			task.IsCompleted.Should().BeFalse();
			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeFalse();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ex);
			}

			scheduler.AdvanceBy(5);

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Faulted);
			task.Exception.Should().NotBeNull();
			var aggregateException = task.Exception;
			aggregateException.InnerExceptions.Should().HaveCount(1);
			var argException = aggregateException.InnerException;
			argException.Should().BeOfType<ArgumentNullException>();
			argException.TargetSite.Name.Should().BeEquivalentTo(nameof(TestSetExceptionAsynchronously), "original context is lost");

			scheduler.Stop();
		}

		[TestMethod]
		public void TestSetExceptionSynchronously_UsingExceptionDispatchInfo1()
		{
			var sut = new TaskCompletionSource<string>();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ex);
			}

			// The task is created AFTER the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Faulted);
			task.Exception.Should().NotBeNull();
			var aggregateException = task.Exception;
			aggregateException.InnerExceptions.Should().HaveCount(1);
			var argException = aggregateException.InnerException;
			argException.Should().BeOfType<ArgumentNullException>();
			argException.TargetSite.Name.Should().BeEquivalentTo(nameof(TestSetExceptionSynchronously_UsingExceptionDispatchInfo1), "original context is lost");
		}

		[TestMethod]
		public void TestSetExceptionSynchronously_UsingExceptionDispatchInfo2()
		{
			var sut = new TaskCompletionSource<string>();

			// The task is created BEFORE the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ex);
			}

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Faulted);
			task.Exception.Should().NotBeNull();
			var aggregateException = task.Exception;
			aggregateException.InnerExceptions.Should().HaveCount(1);
			var argException = aggregateException.InnerException;
			argException.Should().BeOfType<ArgumentNullException>();
			argException.TargetSite.Name.Should().BeEquivalentTo(nameof(TestSetExceptionSynchronously_UsingExceptionDispatchInfo2), "original context is lost");
		}

		[TestMethod]
		public void TestSetExceptionSynchronously_WithTaskCanceledException1()
		{
			var sut = new TaskCompletionSource<string>();

			// The task is created BEFORE the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			try
			{
				throw new TaskCanceledException("xxx");
			}
			catch (TaskCanceledException ex)
			{
				sut.SetException(ex);
			}

			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);
			task.Exception.Should().BeNull();
		}

		[TestMethod]
		public void TestSetExceptionSynchronously_WithTaskCanceledException2()
		{
			var sut = new TaskCompletionSource<string>();

			try
			{
				throw new TaskCanceledException("xxx");
			}
			catch (TaskCanceledException ex)
			{
				sut.SetException(ex);
			}

			// The task is created AFTER the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);
			task.Exception.Should().BeNull();
		}

		[TestMethod]
		public void TestSetResultTwice_ThrowException()
		{
			var sut = new TaskCompletionSource<string>();

			Action action = () => sut.SetResult("1234");

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void TestSetCanceledTwice_ThrowException()
		{
			var sut = new TaskCompletionSource<string>();

			Action action = () => sut.SetCanceled();

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void TestSetExceptionTwice_ThrowException()
		{
			var sut = new TaskCompletionSource<string>();

			Action action = () => sut.SetException(new ArgumentNullException("xxx"));

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public async Task Test()
		{
			var testCtx = GetScheduler();
			var syncCtx = SynchronizationContext.Current;
			var scheCtx = TaskScheduler.Current;

			var tcs = new TaskCompletionSource<object>();

			Task t = default(Task);
			//t = Task.Run(async () =>
			//{
			//	var syncCtx2 = SynchronizationContext.Current;
			//	var scheCtx2 = TaskScheduler.Current;

			//	t.ToString();

			//	await Task.Delay(5000);

			//	tcs.SetResult(default(object));

			//	"".ToString();
			//});

			t = Factory().StartNew(async () =>
			{
				var syncCtx2 = SynchronizationContext.Current;
				var scheCtx2 = TaskScheduler.Current;

				t.ToString();

				//await Blabla();
				//await Blabla2();
				await Blabla3();

				var syncCtx3 = SynchronizationContext.Current;
				var scheCtx3 = TaskScheduler.Current;


				await Task.Delay(50000);

				tcs.SetResult(default(object));

				"".ToString();
			});

			var t2 =  tcs.Task;

			_testScheduler.AdvanceBy(100000 * TimeSpan.TicksPerMillisecond);

			await t2;

			"".ToString();
		}

		private async Task Blabla()
		{
			var syncCtx = SynchronizationContext.Current;
			var scheCtx = TaskScheduler.Current;

			var tcs = new TaskCompletionSource<object>();

			new Thread(Run).Start();

			var result = await tcs.Task;

			result.ToString();

			var syncCtx2 = SynchronizationContext.Current;
			var scheCtx2 = TaskScheduler.Current;

			void Run()
			{
				var syncCtx3 = SynchronizationContext.Current;
				var scheCtx3 = TaskScheduler.Current;

				Thread.Sleep(5000);

				tcs.SetResult(new object());
			}
		}


		private async Task Blabla2()
		{
			var syncCtx = SynchronizationContext.Current;
			var scheCtx = TaskScheduler.Current;

			var buffer = new byte[24];
			int read;
			while((read = await File.OpenRead(@"C:\Users\David.Rey\Desktop\Test.txt").ReadAsync(buffer, 0, 24)) > 0)
			{
				read.ToString();
				Thread.Sleep(100);
			}

			var syncCtx2 = SynchronizationContext.Current;
			var scheCtx2 = TaskScheduler.Current;
		}

		private async Task Blabla3()
		{
			var syncCtx = SynchronizationContext.Current;
			var scheCtx = TaskScheduler.Current;

			var tcs = new TaskCompletionSource<object>();


			_testScheduler.Schedule(async () =>
			{
				tcs.SetResult(new object());
			});

			await tcs.Task;
		}

		private TaskFactory Factory()
		{
			return new TaskFactory(
				CancellationToken.None,
				TaskCreationOptions.AttachedToParent,
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent,
				GetScheduler());
		}

		private class SchedulerTaskScheduler : TaskScheduler
		{
			private readonly IScheduler _scheduler;
			//private SchedulerSynchronizationContext _ctx;

			public SchedulerTaskScheduler(IScheduler scheduler)
			{
				_scheduler = scheduler;
				//_ctx = new SchedulerSynchronizationContext(scheduler);
			}

			/// <inheritdoc />
			protected override void QueueTask(Task task)
			{
				_scheduler.Schedule(task, InvokeTask);
				//_ctx.Post(s_postCallback, task);
			}

			private IDisposable InvokeTask(IScheduler scheduler, Task task)
			{
				var syncCtx = SynchronizationContext.Current;
				var scheCtx = TaskScheduler.Current;

				TryExecuteTask(task);

				return Disposable.Empty;
			}

			/// <inheritdoc />
			protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
			{
				return TryExecuteTask(task);
			}

			/// <inheritdoc />
			protected override IEnumerable<Task> GetScheduledTasks() => throw new NotSupportedException();


			//private static SendOrPostCallback s_postCallback = new SendOrPostCallback(PostCallback);

			//// this is where the actual task invocation occures
			//private static void PostCallback(object obj)
			//{
			//	Task task = (Task)obj;

			//	// calling ExecuteEntry with double execute check enabled because a user implemented SynchronizationContext could be buggy
			//	((IThreadPoolWorkItem)task).RunSynchronously();
			//}
		}

		private TestScheduler _testScheduler;

		private TaskScheduler GetScheduler()
		{
			_testScheduler = new TestScheduler();

			return new SchedulerTaskScheduler(_testScheduler);


			//var ctx = new SchedulerSynchronizationContext(_testScheduler);

			//try
			//{
			//	SynchronizationContext.SetSynchronizationContext(ctx);

			//	return TaskScheduler.FromCurrentSynchronizationContext();
			//}
			//finally
			//{
			//	SynchronizationContext.SetSynchronizationContext(null);
			//}
		}



		[TestMethod]
		public void TestContinuationWhenSetResultUsingAnotherSyncContext()
		{
			var sut = new FastTaskCompletionSource<string>();
			var scheduler = new TestScheduler();
			var threadId = Thread.CurrentThread.ManagedThreadId;

			var isRunning = false;
			var error = default(Exception);

			var ctx = new ErrorSyncContext();

			scheduler.Schedule(async () =>
			{
				isRunning = true;
				try
				{
					//new TaskFactory(TaskScheduler.Default);

					//Task.Factory

					//new Task(default(Action), CancellationToken.None, )

					//TaskScheduler.FromCurrentSynchronizationContext().

					//Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);

					SynchronizationContext.SetSynchronizationContext(ctx);
					var task = sut.Task;

					await task;
					SynchronizationContext.SetSynchronizationContext(null);

					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
				}
				catch (Exception e)
				{
					error = e;
				}
				finally
				{
					isRunning = false;
				}
			});
			scheduler.Schedule(() =>
			{
				isRunning.Should().BeTrue();

				SynchronizationContext.SetSynchronizationContext(ctx);
				sut.SetResult("1234");

				isRunning.Should().BeFalse();
				if (error != null)
				{
					ExceptionDispatchInfo.Capture(error).Throw();
				}
			});

			scheduler.AdvanceBy(100);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestContinuationWhenSetExceptionUsingAnotherSyncContext()
		{
			var sut = new FastTaskCompletionSource<string>();
			var scheduler = new TestScheduler();
			var threadId = Thread.CurrentThread.ManagedThreadId;

			var isRunning = false;
			var error = default(Exception);

			scheduler.Schedule(async () =>
			{
				isRunning = true;
				try
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);

					await sut.Task;

					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
				}
				catch (Exception e)
				{
					error = e;
				}
				finally
				{
					isRunning = false;
				}
			});
			scheduler.Schedule(() =>
			{
				isRunning.Should().BeTrue();

				SynchronizationContext.SetSynchronizationContext(new ErrorSyncContext());
				sut.SetException(new ArgumentNullException("xxx"));

				isRunning.Should().BeFalse();
				if (error != null)
				{
					ExceptionDispatchInfo.Capture(error).Throw();
				}
			});

			scheduler.AdvanceBy(100);
		}

		[TestMethod]
		[ExpectedException(typeof(OperationCanceledException))]
		public void TestContinuationWhenSetCancelUsingAnotherSyncContext()
		{
			var sut = new FastTaskCompletionSource<string>();
			var scheduler = new TestScheduler();
			var threadId = Thread.CurrentThread.ManagedThreadId;

			var isRunning = false;
			var error = default(Exception);

			scheduler.Schedule(async () =>
			{
				isRunning = true;
				try
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);

					await sut.Task;

					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
				}
				catch (Exception e)
				{
					error = e;
				}
				finally
				{
					isRunning = false;
				}
			});
			scheduler.Schedule(() =>
			{
				isRunning.Should().BeTrue();

				SynchronizationContext.SetSynchronizationContext(new ErrorSyncContext());
				sut.SetCanceled();

				isRunning.Should().BeFalse();
				if (error != null)
				{
					ExceptionDispatchInfo.Capture(error).Throw();
				}
			});

			scheduler.AdvanceBy(100);
		}

		private class ErrorSyncContext : SynchronizationContext
		{
			public override void Post(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("Cannot schedule anything on the ErrorSyncContext");
			}

			public override void Send(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("Cannot schedule anything on the ErrorSyncContext");
			}
		}
	}
}
