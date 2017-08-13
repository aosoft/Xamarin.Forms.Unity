using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	/// <summary>
	/// Xamarin.Forms の Forms 実装。
	/// 通常の実装と異なり、どこかの GameObject に AddComponent しておくことが Forms.Init 相当の処置になる。 
	/// </summary>
	public class Forms : MonoBehaviour
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Thread _mainThread;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			_mainThread = Thread.CurrentThread;
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		public Thread MainThread => _mainThread;

		#endregion
	}
}
