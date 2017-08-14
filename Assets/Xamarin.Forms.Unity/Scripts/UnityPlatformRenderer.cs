using System.Collections;
using System.Collections.Generic;
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

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Start()
		{
			
		}

		#endregion
	}
}
