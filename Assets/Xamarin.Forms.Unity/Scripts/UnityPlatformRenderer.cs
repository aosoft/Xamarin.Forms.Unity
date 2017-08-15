using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	/// <summary>
	/// Xamarin.Forms の PlatformRenderer 実装。
	/// 通常の実装と異なり、ルートの Canvas に AddComponent する想定。
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	public class UnityPlatformRenderer : MonoBehaviour
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		UnityPlatform _platform;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			_platform = new UnityPlatform();
		}

		private void OnDestroy()
		{
			_platform = null;
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Public Method

		public void LoadApplication(Application app)
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		void OnApplicationPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		#endregion
	}
}
