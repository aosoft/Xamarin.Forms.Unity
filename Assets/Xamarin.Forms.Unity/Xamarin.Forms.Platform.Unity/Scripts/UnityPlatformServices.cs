using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    public class UnityPlatformServices : IPlatformServices
    {
        /*-----------------------------------------------------------------*/

        #region Private Field

        private static readonly MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

        #endregion Private Field

        /*-----------------------------------------------------------------*/

        #region Constructor

        public UnityPlatformServices()
        {
        }

        #endregion Constructor

        /*-----------------------------------------------------------------*/

        #region IPlatformServices

        public bool IsInvokeRequired => Forms.MainThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;

        public string RuntimePlatform => "Unity";

        public void BeginInvokeOnMainThread(Action action)
        {
            SynchronizationContext.Current.Post(_ => action(), null);
        }

        public Ticker CreateTicker()
        {
            return new UnityTicker();
        }

        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public string GetMD5Hash(string input)
        {
            var bytes = _md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var c in bytes)
            {
                sb.AppendFormat("{0:X2}", c);
            }
            return sb.ToString();
        }

        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            return 10.0;
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            return null;
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            return new UnityIsolatedStorage();
        }

        public void OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            Forms.Activity.StartCoroutine(TimerCoroutine((float)interval.TotalSeconds, callback));
        }

        private IEnumerator TimerCoroutine(float seconds, Func<bool> callback)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                if (callback() == false)
                {
                    break;
                }
            }
        }

        public void QuitApplication()
        {
            UnityEngine.Application.Quit();
        }

        #endregion IPlatformServices
    }
}