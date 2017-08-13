using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class UnityPlatformServices : IPlatformServices
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Thread _mainThread;

		static readonly MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor

		public UnityPlatformServices(Thread mainThread)
		{
			_mainThread = mainThread;
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region IPlatformServices

		public bool IsInvokeRequired => _mainThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;

		public string RuntimePlatform => "Unity";

		public void BeginInvokeOnMainThread(Action action)
		{
			SynchronizationContext.Current.Post(_ => action(), null);
		}

		public Ticker CreateTicker()
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public void OpenUriAction(Uri uri)
		{
			throw new NotImplementedException();
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			//UnityEngine.GameObject.St
		}

		#endregion
	}
}