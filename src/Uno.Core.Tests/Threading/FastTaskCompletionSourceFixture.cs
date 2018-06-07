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
using System.Reactive.Concurrency;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					var r = await sut;
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
			var sut = new FastTaskCompletionSource<string>();

			var threadId = Thread.CurrentThread.ManagedThreadId;

			sut.SetResult("1234");

			scheduler.AdvanceBy(5);

			var task = scheduler
				.Run(async ct =>
				{
					Thread.CurrentThread.ManagedThreadId.Should().Be(threadId);
					var r = await sut;
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
			var sut = new FastTaskCompletionSource<string>();

			sut.SetCanceled();

			// The task is created AFTER the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeTrue();
			task.Status.Should().Be(TaskStatus.Canceled);
		}

		[TestMethod]
		public void TestSetCanceledSynchronously2()
		{
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ExceptionDispatchInfo.Capture(ex));
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
			var sut = new FastTaskCompletionSource<string>();

			// The task is created BEFORE the task being canceled
			var task = sut.Task;
			task.IsCompleted.Should().BeFalse();

			try
			{
				throw new ArgumentNullException("xxx");
			}
			catch (ArgumentNullException ex)
			{
				sut.SetException(ExceptionDispatchInfo.Capture(ex));
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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

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
			var sut = new FastTaskCompletionSource<string>();

			Action action = () => sut.SetResult("1234");

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void TestSetCanceledTwice_ThrowException()
		{
			var sut = new FastTaskCompletionSource<string>();

			Action action = () => sut.SetCanceled();

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void TestSetExceptionTwice_ThrowException()
		{
			var sut = new FastTaskCompletionSource<string>();

			Action action = () => sut.SetException(new ArgumentNullException("xxx"));

			action.Should().NotThrow();
			action.Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void TestContinuationWhenSetResultUsingAnotherSyncContext()
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
