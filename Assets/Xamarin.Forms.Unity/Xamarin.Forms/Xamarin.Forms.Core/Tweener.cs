//
// Tweener.cs
//
// Author:
//       Jason Smith <jason.smith@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	internal class Tweener
	{
		private long _lastMilliseconds;

		private int _timer;

		public Tweener(uint length)
		{
			Value = 0.0f;
			Length = length;
			Loop = false;
		}

		public AnimatableKey Handle { get; set; }

		public uint Length { get; }

		public bool Loop { get; set; }

		public double Value { get; private set; }

		public event EventHandler Finished;

		public void Pause()
		{
			if (_timer != 0)
			{
				Ticker.Default.Remove(_timer);
				_timer = 0;
			}
		}

		public void Start()
		{
			Pause();

			_lastMilliseconds = 0;
			_timer = Ticker.Default.Insert(step =>
			{
				long ms = step + _lastMilliseconds;

				Value = Math.Min(1.0f, ms / (double)Length);

				_lastMilliseconds = ms;

				if (ValueUpdated != null)
					ValueUpdated(this, EventArgs.Empty);

				if (Value >= 1.0f)
				{
					if (Loop)
					{
						_lastMilliseconds = 0;
						Value = 0.0f;
						return true;
					}

					if (Finished != null)
						Finished(this, EventArgs.Empty);
					Value = 0.0f;
					_timer = 0;
					return false;
				}
				return true;
			});
		}

		public void Stop()
		{
			Pause();
			Value = 1.0f;
			if (Finished != null)
				Finished(this, EventArgs.Empty);
			Value = 0.0f;
		}

		public event EventHandler ValueUpdated;

		~Tweener()
		{
			if (_timer != 0)
			{
				try
				{
					Ticker.Default.Remove(_timer);
				}
				catch (InvalidOperationException)
				{
				}
			}
			_timer = 0;
		}
	}
}