using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	internal class LockingSemaphore
	{
		private static readonly Task Completed = Task.FromResult(true);
		private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
		private int _currentCount;

		public LockingSemaphore(int initialCount)
		{
			if (initialCount < 0)
				throw new ArgumentOutOfRangeException("initialCount");
			_currentCount = initialCount;
		}

		public void Release()
		{
			TaskCompletionSource<bool> toRelease = null;
			lock (_waiters)
			{
				if (_waiters.Count > 0)
					toRelease = _waiters.Dequeue();
				else
					++_currentCount;
			}
			if (toRelease != null)
				toRelease.TrySetResult(true);
		}

		public Task WaitAsync(CancellationToken token)
		{
			lock (_waiters)
			{
				if (_currentCount > 0)
				{
					--_currentCount;
					return Completed;
				}
				var waiter = new TaskCompletionSource<bool>();
				_waiters.Enqueue(waiter);
				token.Register(() => waiter.TrySetCanceled());
				return waiter.Task;
			}
		}
	}
}