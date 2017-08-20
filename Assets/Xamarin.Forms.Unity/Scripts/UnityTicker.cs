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
		/*-----------------------------------------------------------------*/
		#region Private Field

		IEnumerator _coroutine = null;

		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor

		public UnityTicker()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Ticker

		protected override void DisableTimer()
		{
			if (_coroutine != null)
			{
				Forms.Activity.StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}

		protected override void EnableTimer()
		{
			if (_coroutine == null)
			{
				_coroutine = TimerCorutine();
				Forms.Activity.StartCoroutine(_coroutine);
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

		#endregion

	}
}
