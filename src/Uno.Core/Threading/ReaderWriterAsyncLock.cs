using System;
using System.Collections.Generic;
using System.Text;
using Uno.Disposables;
using Uno.Threading;

namespace Uno.Core.Threading
{
	/// <summary>
	/// An <see cref="FastAsyncLock"/> that allow concurrent access for read operation,
	/// but sequential access for write operation.
	/// </summary>
	/// <remarks>
	/// This implementation won't queue read operations if somme writers are waiting. This means that a write operation
	/// may never acquire the lock if there is continuously new read operations that acquire the lock be fore all read operations are over.
	/// </remarks>
	public sealed class ReaderWriterAsyncLock
	{
		private readonly FastAsyncLock _runningGate = new FastAsyncLock();
		private readonly object _enterGate = new object();

		private int _readers = 0;

		private Task<IDisposable> _acquireRead;

		/// <summary>
		/// Acquires the lock for concurrent reads
		/// </summary>
		/// <param name="ct">A <see cref="CancellationToken"/> to abort the acquire</param>
		/// <returns>An asynchronous token to release the lock</returns>
		public async Task<IDisposable> AcquireRead(CancellationToken ct)
			=> Acquire(ct, allowConcurrent: true);

		/// <summary>
		/// Acquires the lock for sequential write
		/// </summary>
		/// <param name="ct">A <see cref="CancellationToken"/> to abort the acquire</param>
		/// <returns>An asynchronous token to release the lock</returns>
		public async Task<IDisposable> AcquireWrite(CancellationToken ct)
			=> Acquire(ct, allowConcurrent: false);

		private async Task<IDisposable> Acquire(CancellationToken ct, bool allowConcurrent)
		{
			Task<IDisposable> acquire;
			lock (_enterGate)
			{
				if (allowConcurrent)
				{
					if (Interlocked.Increment(ref _readers) == 1)
					{
						_acquireRead = _runningGate.LockAsync(CancellationToken.None);
					}

					acquire = _acquireRead;
				}
				else
				{
					acquire = _runningGate.LockAsync(ct);
				}
			}

			var token = await acquire;

			if (allowConcurrent)
			{
				return Disposable.Create(Release);

				void Release()
				{
					if (Interlocked.Decrement(ref _readers) == 0)
					{
						token.Dispose();

						// Try to cleanup, but make sure to not erase the lock if it has already been re-acquired
						Interlocked.CompareExchange(ref _acquireRead, null, acquire);
					}
				}
			}
			else
			{
				return token;
			}
		}
	}
}
