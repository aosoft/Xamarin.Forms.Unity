using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class UnityTicker : Ticker
	{
		MonoBehaviour _behavior = null;
		IEnumerator _coroutine = null;

		public UnityTicker(MonoBehaviour mb)
		{
			_behavior = mb;
		}

		protected override void DisableTimer()
		{
			if (_coroutine != null)
			{
				_behavior.StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}

		protected override void EnableTimer()
		{
			if (_coroutine == null)
			{
				_coroutine = TimerCorutine();
				_behavior.StartCoroutine(_coroutine);
			}
		}

		IEnumerator TimerCorutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.01f);
				SendSignals();
			}
		}
	}
}
