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
	/// Root Canvas は UnityFormsApplicationActivity に指定しておく。
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	public class UnityPlatformRenderer : MonoBehaviour
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Canvas _canvas;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Start()
		{
			_canvas = GetComponent<Canvas>();
		}

		private void OnDestroy()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Public Method

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		#endregion
	}
}
