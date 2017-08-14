using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	/// <summary>
	/// Xamarin.Forms の初期化をする MonoBehavior。
	/// どこかの UI EventSystem に AddComponent しておくことが Forms.Init 相当の処置になる。 
	/// </summary>
	[DisallowMultipleComponent]
	public class FormsInitializer<T> : MonoBehaviour
		where T : Application, new()
	{
		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			Forms.Init(this);
		}

		private void OnDestroy()
		{
			Forms.Uninit();
		}

		#endregion

	}
}
