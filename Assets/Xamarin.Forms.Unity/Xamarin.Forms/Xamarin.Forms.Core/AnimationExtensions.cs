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
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public static class AnimationExtensions
	{
		private static readonly Dictionary<AnimatableKey, Info> s_animations;
		private static readonly Dictionary<AnimatableKey, int> s_kinetics;

		static AnimationExtensions()
		{
			s_animations = new Dictionary<AnimatableKey, Info>();
			s_kinetics = new Dictionary<AnimatableKey, int>();
		}

		public static bool AbortAnimation(this IAnimatable self, string handle)
		{
			var key = new AnimatableKey(self, handle);

			if (!s_animations.ContainsKey(key) && !s_kinetics.ContainsKey(key))
			{
				return false;
			}

			Action abort = () =>
			{
				AbortAnimation(key);
				AbortKinetic(key);
			};

			if (Device.IsInvokeRequired)
			{
				Device.BeginInvokeOnMainThread(abort);
			}
			else
			{
				abort();
			}

			return true;
		}

		public static void Animate(this IAnimatable self, string name, Animation animation, uint rate = 16, uint length = 250, Easing easing = null, Action<double, bool> finished = null,
								   Func<bool> repeat = null)
		{
			if (repeat == null)
				self.Animate(name, animation.GetCallback(), rate, length, easing, finished, null);
			else
			{
				Func<bool> r = () =>
				{
					var val = repeat();
					if (val)
						animation.ResetChildren();
					return val;
				};
				self.Animate(name, animation.GetCallback(), rate, length, easing, finished, r);
			}
		}

		public static void Animate(this IAnimatable self, string name, Action<double> callback, double start, double end, uint rate = 16, uint length = 250, Easing easing = null,
								   Action<double, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate(name, Interpolate(start, end), callback, rate, length, easing, finished, repeat);
		}

		public static void Animate(this IAnimatable self, string name, Action<double> callback, uint rate = 16, uint length = 250, Easing easing = null, Action<double, bool> finished = null,
								   Func<bool> repeat = null)
		{
			self.Animate(name, x => x, callback, rate, length, easing, finished, repeat);
		}

		public static void Animate<T>(this IAnimatable self, string name, Func<double, T> transform, Action<T> callback,
			uint rate = 16, uint length = 250, Easing easing = null,
			Action<T, bool> finished = null, Func<bool> repeat = null)
		{
			if (transform == null)
				throw new ArgumentNullException(nameof(transform));
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));
			if (self == null)
				throw new ArgumentNullException(nameof(self));

			Action animate = () => AnimateInternal(self, name, transform, callback, rate, length, easing, finished, repeat);

			if (Device.IsInvokeRequired)
			{
				Device.BeginInvokeOnMainThread(animate);
			}
			else
			{
				animate();
			}
		}

		public static void AnimateKinetic(this IAnimatable self, string name, Func<double, double, bool> callback, double velocity, double drag, Action finished = null)
		{
			Action animate = () => AnimateKineticInternal(self, name, callback, velocity, drag, finished);

			if (Device.IsInvokeRequired)
			{
				Device.BeginInvokeOnMainThread(animate);
			}
			else
			{
				animate();
			}
		}

		public static bool AnimationIsRunning(this IAnimatable self, string handle)
		{
			var key = new AnimatableKey(self, handle);
			return s_animations.ContainsKey(key);
		}

		public static Func<double, double> Interpolate(double start, double end = 1.0f, double reverseVal = 0.0f, bool reverse = false)
		{
			double target = reverse ? reverseVal : end;
			return x => start + (target - start) * x;
		}

		private static void AbortAnimation(AnimatableKey key)
		{
			if (!s_animations.ContainsKey(key))
			{
				return;
			}

			Info info = s_animations[key];
			info.Tweener.ValueUpdated -= HandleTweenerUpdated;
			info.Tweener.Finished -= HandleTweenerFinished;
			info.Tweener.Stop();
			info.Finished?.Invoke(1.0f, true);

			s_animations.Remove(key);
		}

		private static void AbortKinetic(AnimatableKey key)
		{
			if (!s_kinetics.ContainsKey(key))
			{
				return;
			}

			Ticker.Default.Remove(s_kinetics[key]);
			s_kinetics.Remove(key);
		}

		private static void AnimateInternal<T>(IAnimatable self, string name, Func<double, T> transform, Action<T> callback,
			uint rate, uint length, Easing easing, Action<T, bool> finished, Func<bool> repeat)
		{
			var key = new AnimatableKey(self, name);

			AbortAnimation(key);

			Action<double> step = f => callback(transform(f));
			Action<double, bool> final = null;
			if (finished != null)
				final = (f, b) => finished(transform(f), b);

			var info = new Info { Rate = rate, Length = length, Easing = easing ?? Easing.Linear };

			var tweener = new Tweener(info.Length);
			tweener.Handle = key;
			tweener.ValueUpdated += HandleTweenerUpdated;
			tweener.Finished += HandleTweenerFinished;

			info.Tweener = tweener;
			info.Callback = step;
			info.Finished = final;
			info.Repeat = repeat;
			info.Owner = new WeakReference<IAnimatable>(self);

			s_animations[key] = info;

			info.Callback(0.0f);
			tweener.Start();
		}

		private static void AnimateKineticInternal(IAnimatable self, string name, Func<double, double, bool> callback, double velocity, double drag, Action finished = null)
		{
			var key = new AnimatableKey(self, name);

			AbortKinetic(key);

			double sign = velocity / Math.Abs(velocity);
			velocity = Math.Abs(velocity);

			int tick = Ticker.Default.Insert(step =>
			{
				long ms = step;

				velocity -= drag * ms;
				velocity = Math.Max(0, velocity);

				var result = false;
				if (velocity > 0)
				{
					result = callback(sign * velocity * ms, velocity);
				}

				if (!result)
				{
					finished?.Invoke();
					s_kinetics.Remove(key);
				}
				return result;
			});

			s_kinetics[key] = tick;
		}

		private static void HandleTweenerFinished(object o, EventArgs args)
		{
			var tweener = o as Tweener;
			Info info;
			if (tweener != null && s_animations.TryGetValue(tweener.Handle, out info))
			{
				IAnimatable owner;
				if (info.Owner.TryGetTarget(out owner))
					owner.BatchBegin();
				info.Callback(tweener.Value);

				var repeat = false;
				if (info.Repeat != null)
					repeat = info.Repeat();

				if (!repeat)
				{
					s_animations.Remove(tweener.Handle);
					tweener.ValueUpdated -= HandleTweenerUpdated;
					tweener.Finished -= HandleTweenerFinished;
				}

				info.Finished?.Invoke(tweener.Value, false);

				if (info.Owner.TryGetTarget(out owner))
					owner.BatchCommit();

				if (repeat)
				{
					tweener.Start();
				}
			}
		}

		private static void HandleTweenerUpdated(object o, EventArgs args)
		{
			var tweener = o as Tweener;
			Info info;
			IAnimatable owner;

			if (tweener != null && s_animations.TryGetValue(tweener.Handle, out info) && info.Owner.TryGetTarget(out owner))
			{
				owner.BatchBegin();
				info.Callback(info.Easing.Ease(tweener.Value));
				owner.BatchCommit();
			}
		}

		private class Info
		{
			public Action<double> Callback;
			public Action<double, bool> Finished;
			public Func<bool> Repeat;
			public Tweener Tweener;

			public Easing Easing { get; set; }

			public uint Length { get; set; }

			public WeakReference<IAnimatable> Owner { get; set; }

			public uint Rate { get; set; }
		}
	}
}