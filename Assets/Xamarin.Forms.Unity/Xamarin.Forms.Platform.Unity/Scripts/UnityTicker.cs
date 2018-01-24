using System.Collections;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    public class UnityTicker : Ticker
    {
        /*-----------------------------------------------------------------*/

        #region Private Field

        private IEnumerator _coroutine = null;

        #endregion Private Field

        /*-----------------------------------------------------------------*/

        #region Constructor

        public UnityTicker()
        {
        }

        #endregion Constructor

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

        private IEnumerator TimerCorutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);
                SendSignals();
            }
        }

        #endregion Ticker
    }
}